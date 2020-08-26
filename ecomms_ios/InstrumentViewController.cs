using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using UIKit;
using Xamarin.Essentials;

namespace ecomms_ios
{
    public partial class InstrumentViewController : UIViewController, IObserverEx
    {
        public class GetResponse
        {
            public Dictionary<String, String> TartGet { get; set; }

            public String Key()
            {
                String _return = null;
                foreach (String key in TartGet.Keys)
                {
                    _return = key;
                    break;
                }

                return _return;

            }

            public String Value()
            {
                return TartGet[Key()];
            }

            public String Value(String which)
            {
                return TartGet[which];
            }
        }

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

            Title = sensor.name;

            _name.Text = sensor.name;
            _nameLabel.Text = "Instrument name";

            _locationLabel.Text = "Instrument location";
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

