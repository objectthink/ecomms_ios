using System;
using System.Text.Json;
using ECOMMS_Client;
using UIKit;

namespace ecomms_ios
{
    public partial class SensorViewController : UIViewController
    {
        public class SensorDataPoint
        {
            public int temperature { get; set; }
            public int humidity { get; set; }
            public float level { get; set; }
        }

        public SensorData sensor { get; set; }

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
                Console.WriteLine(response);
            });
        }
    }
}

