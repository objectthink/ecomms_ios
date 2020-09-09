using System;

using UIKit;

namespace ecomms_ios
{
    public partial class InstrumentControlViewController : UIViewController, IDataController
    {
        public SensorData sensor
        {
            get;
            set;
        }

        public InstrumentControlViewController() : base("InstrumentControlViewController", null)
        {
        }

        public InstrumentControlViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

