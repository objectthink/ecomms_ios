using System;
using System.Text;
using System.Text.Json;
using ECOMMS_Client;
using ECOMMS_Entity;
using UIKit;
using Xamarin.Essentials;

namespace ecomms_ios
{
    public partial class SensorViewController : UIViewController, IObserverEx
    {
        public class SensorDataPoint
        {
            public int temperature { get; set; }
            public int humidity { get; set; }
            public float level { get; set; }
        }

        public SensorData sensor
        {
            get;
            set;
        }

        public SensorViewController() : base("SensorViewController", null)
        {
        }

        public SensorViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Title = sensor.name;
            _name.Text = sensor.name;
            _location.Text = sensor.location;
            _high.Text = sensor.high;
            _low.Text = sensor.low;

            //json
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            //var data = JsonSerializer.Parse<SensorDataPoint>(json, options);
            SensorDataPoint sdp = JsonSerializer.Deserialize<SensorDataPoint>(sensor.description);

            //parse description json...
            _temperature.Text = sdp.temperature.ToString();
            _humidity.Text = sdp.humidity.ToString();

            //start listening to client
            sensor.client.addObserver(this);

            //add a bound status listener
            sensor.client.addStatusListener(this, (name, bytes) =>
            {
                string description = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                SensorDataPoint json = JsonSerializer.Deserialize<SensorDataPoint>(sensor.description);

                //parse description json...
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _temperature.Text = json.temperature.ToString();
                    _humidity.Text = json.humidity.ToString();
                });
            });
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            //start listening to client
            sensor.client.removeObserver(this);
            sensor.client.removeListener(this);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void SetNameButton_TouchUpInside(UIButton sender)
        {
            sensor.client.doSet("name", _name.Text, (response) => {
                Console.WriteLine(response);
            });
        }

        partial void SetLocationButton_TouchUpInside(UIButton sender)
        {
            sensor.client.doSet("location", _location.Text, (response) => {

                //COULD UPDATE THE CLIENTS LOCATION HERE

                Console.WriteLine(response);
            });
        }

        public void update(IObservableEx observable, object hint, object data)
        {
            //TODO THIS WILL BE JSON IN THE FUTURE
            if((hint as string).Equals("PROPERTY_CHANGED"))
            {
                //get the name and location
                //get the location
                sensor.client.doGet("location", (response) =>
                {
                    //AS WE HAVE NOT CREATED A DERIVED CLASS YET
                    //UPDATE OUR LOCATION HERE
                    sensor.location = response;
                });
                sensor.client.doGet("name", (response) =>
                {
                    //AS WE HAVE NOT CREATED A DERIVED CLASS YET
                    //UPDATE OUR LOCATION HERE
                    sensor.name = response;
                });
            }
        }
    }
}

