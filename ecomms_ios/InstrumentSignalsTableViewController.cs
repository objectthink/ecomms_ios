using System;
using ECOMMS_Client;
using UIKit;
using Xamarin.Essentials;

namespace ecomms_ios
{
    public partial class InstrumentSignalsTableViewController : UITableViewController, IDataController
    {
        public SensorData sensor
        {
            get;
            set;
        }

        public InstrumentSignalsTableViewController() : base("InstrumentSignalsTableViewController", null)
        {
        }

        public InstrumentSignalsTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Title = "Signals";
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            IClient client = sensor.client;

            client.addStatusListener("CONTROL_PAGE", (name, bytes) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TableView.ReloadData();
                });
            });
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            sensor.client.removeListener("CONTROL_PAGE");
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

