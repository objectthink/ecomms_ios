using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ECOMMS_Client;
using Foundation;
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

        public class RealtimeStatus
        {

            public List<SignalObject> Signals { get; set; }
            public String StatusType { get; set; }
        }

        public class SignalSource : UITableViewSource
        {
            public SignalSource()
            {
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                // in a Storyboard, Dequeue will ALWAYS return a cell, 
                var cell = tableView.DequeueReusableCell("SignalItem");

                // now set the properties as normal
                cell.DetailTextLabel.Text = "1";
                cell.TextLabel.Text = "1";

                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return 1;
            }
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

            TableView.Source = new SignalSource();
            //TableView.Delegate = null;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            IClient client = sensor.client;

            client.addStatusListener("CONTROL_PAGE", (name, bytes) =>
            {
                string status = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                //LIST FOR INSTRUMENT STATUS STATUS HERE

                //Console.WriteLine("{0}:status listener:{1}:{2}",
                //    client.name,
                //    name,
                //    status
                //    );

                //which status is this?
                RawStatus r = JsonSerializer.Deserialize<RawStatus>(status);

                if (r.StatusType.Equals("RealTime"))
                {
                    RealtimeStatus rs = JsonSerializer.Deserialize<RealtimeStatus>(status);

                    foreach (SignalObject signal in rs.Signals)
                    {
                        switch (signal.Name)
                        {
                            default:
                                Console.WriteLine(signal.Name);
                                TableView.ReloadData();
                                break;
                        }
                    }
                }

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

