using System;
using System.Text;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using UIKit;
using Xamarin.Essentials;

namespace ecomms_ios
{
    public partial class InstrumentViewController : UIViewController, IObserverEx
    {
        public SensorData sensor
        {
            get;
            set;
        }

        public InstrumentViewController() : base("InstrumentViewController", null)
        {
        }

        public InstrumentViewController(IntPtr handle) : base(handle)
        {
        }

        public void update(IObservableEx observable, object hint, object data)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Title = "an instrument";
            _name.Text = sensor.name;
            _location.Text = sensor.location;

            //add a bound status listener
            sensor.client.addStatusListener(this, (name, bytes) =>
            {
                string description = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                string cleaned = description.Replace("\n", "").Replace("\r", "");

                //parse description json...
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _status.Text = cleaned;
                });
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

