using System;
using UIKit;

namespace ecomms_ios
{
    public partial class SensorViewController : UIViewController
    {
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
            
            //parse description json...
            _temperature.Text = "coming!";
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

