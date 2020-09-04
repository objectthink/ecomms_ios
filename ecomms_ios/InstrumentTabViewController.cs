using System;
using Foundation;
using UIKit;

namespace ecomms_ios
{
    public partial class InstrumentTabViewController : UITabBarController
    {
        public class InstrumentTabViewControllerDelegate : UITabBarControllerDelegate
        {
        }

        public SensorData sensor
        {
            get;
            set;
        }

        public InstrumentTabViewController() : base("InstrumentTabViewController", null)
        {
        }

        public InstrumentTabViewController(IntPtr handle) : base(handle)
        {
           
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            foreach(IDataController controller in ViewControllers)
            {
                controller.sensor = sensor;
            }

            ViewControllerSelected += (sender, e) =>
            {
                Console.WriteLine("VIEW SELECTED");
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
        }
    }
}

