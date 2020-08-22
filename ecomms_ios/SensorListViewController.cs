using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;

namespace ecomms_ios
{
    public class SensorData
    {
        public String name { get; set; }
        public String temperature { get; set; }
        public String description { get; set; }
        public String location { get; set; }
        public String high { get; set; }
        public String low { get; set; }

        public IClient client { get; set; }

        public SensorData()
        {
        }
    }

    //SensorClient
    //ecoms sensor client derived
    public class SensorClient : Client
    {
        public SensorClient(string id, Role role, ECOMMS_Entity.Type type) : base(id, role, type)
        {
        }
    }

    public partial class SensorListViewController : UITableViewController, IClientFactory
    {
        List<string> _sensorNames = new List<string>();
        List<SensorData> _sensorDataList = new List<SensorData>();
        Dictionary<string, SensorData> _sensorDictionary = new Dictionary<string, SensorData>();

        UIRefreshControl _refreshControl = new UIRefreshControl();

        //ECOMMS Manager
        Manager _manager;

        //enables us to populate the manager with derived clients that we create here
        public IClient getClientFor(string address, Role role, ECOMMS_Entity.Type type, SubType subType)
        {
            return new SensorClient(address, role, type);
        }

        public class SensorSource : UITableViewSource
        {
            List<string> _sensorNames = new List<string>();
            Dictionary<string, SensorData> _sensorDictionary = new Dictionary<string, SensorData>();

            public SensorSource(List<string> sensorNames, Dictionary<string, SensorData> sensorDictionary)
            {
                _sensorNames = sensorNames;
                _sensorDictionary = sensorDictionary;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                // in a Storyboard, Dequeue will ALWAYS return a cell, 
                var cell = tableView.DequeueReusableCell("SensorItem");

                // now set the properties as normal
                cell.DetailTextLabel.Text = _sensorDictionary[_sensorNames[indexPath.Row]].name;
                cell.TextLabel.Text = _sensorDictionary[_sensorNames[indexPath.Row]].description;

                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return _sensorNames.Count;
            }
        }

        public class SensorDelegate : UITableViewDelegate
        {
            SensorListViewController _controller;
            public SensorDelegate(SensorListViewController controller) : base()
            {
                _controller = controller;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                Console.WriteLine("ROW SELECTED {0}", indexPath.Row);

                string name = _controller._sensorNames.ToArray()[indexPath.Row];
                if(_controller._sensorDictionary[name].client.role == Role.Sensor)
                {
                    _controller.PerformSegue("sensorSegue", this);
                }

                if (_controller._sensorDictionary[name].client.role == Role.Instrument)
                {
                    _controller.PerformSegue("instrumentSegue", this);
                }
            }
        }

        public SensorListViewController() : base("SensorListViewController", null)
        {
        }

        public SensorListViewController(IntPtr handle) : base(handle)
        {
        }

        private void addInstrument(IClient client)
        {
            addSensor(client);
        }

        //add a sensor to our list of sensors
        private void addSensor(IClient client)
        {
            if (client.role == Role.Sensor || client.role == Role.Instrument)
            {

                Console.WriteLine(client.name + " SENSOR ADDED");

                if (!_sensorNames.Contains(client.name))
                {
                    _sensorNames.Add(client.name);
                    _sensorDataList.Add(new SensorData());
                    _sensorDictionary.Add(client.name, new SensorData());

                    _sensorDictionary[client.name].client = client;

                    //get the location
                    client.doGet("location", (response) =>
                    {
                        _sensorDictionary[client.name].location = response;
                    });

                    //get the low
                    client.doGet("low", (response) =>
                    {
                        _sensorDictionary[client.name].low = response;
                    });

                    //get the high
                    client.doGet("high", (response) =>
                    {
                        _sensorDictionary[client.name].high = response;
                    });

                    //listen for run state changes
                    client.addObserver(new ObserverAdapterEx((anobject, hint, data) =>
                    {
                        Console.WriteLine((hint as string));
                    }));

                    client.addObserver(new ObserverAdapter((observable, hint) =>
                    {
                        String notification = hint as String;

                        Console.WriteLine((hint as string));

                        if (hint.Equals("ONLINE_CHANGED"))
                        {
                            IClient me = observable as IClient;

                            if (!me.online)
                            {
                                _ = _sensorNames.Remove(me.name);
                                _ = _sensorDataList.Remove(_sensorDictionary[me.name]);
                                _ = _sensorDictionary.Remove(me.name);

                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    TableView.ReloadData();
                                });

                                ///////////////////////////////////////////////
                                ///send notification - sensor offline
                                var content = new UNMutableNotificationContent();
                                content.Title = "ECOMMS iOS";
                                content.Subtitle = "Sensor offline!";
                                content.Body = me.name + " went offline";
                                content.Badge = 1;

                                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);

                                var requestID = "sampleRequest";
                                var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

                                UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => {
                                    if (err != null)
                                    {
                                        // Do something with error...
                                    }
                                });
                            }
                        }
                    }));

                    //add a status listener
                    client.addStatusListener((name, bytes) =>
                    {
                        Console.WriteLine("{0}:status listener:{1}:{2}",
                            client.name,
                            name,
                            Encoding.UTF8.GetString(bytes, 0, bytes.Length)
                            );

                        _sensorDictionary[client.name].description = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        _sensorDictionary[client.name].name = client.name;

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            TableView.ReloadData();
                        });
                    });
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            TableView.Source = new SensorSource(_sensorNames, _sensorDictionary);
            TableView.Delegate = new SensorDelegate(this);

            //SETUP PULL TO REFRESH
            //USE TO REFRESH THE LIST OF SENSORS
            _refreshControl.ValueChanged += (sender, args) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _refreshControl.EndRefreshing();
                    _sensorDictionary.Clear();
                    _sensorDataList.Clear();
                    _sensorNames.Clear();

                    foreach (IClient client in _manager.clients)
                        addSensor(client);

                    TableView.ReloadData();
                });
            };

            TableView.AddSubview(_refreshControl);
            ////////////////////////

            //SETUP ECOMMS MANAGER AND START LISTENING TO CLIENT LIST CHANGES
            _manager = new Manager();

            //consider supporting nats list
            _manager.connect(@"nats://192.168.86.31:7222"); //.27 rPi, .30 maclinbook
            _manager.init();

            //addobserver(observerex) notifies with data which is the added client in this case
            _manager.addObserver(new ObserverAdapterEx((o, h, c) =>
            {
                //need to wait to notify until after base class has gotton response
                //to role request
                //or have library query first before creating client
                //WIP...

                var client = c as IClient;
                switch (h)
                {
                    case "CONNECTED":

                        if (client.role == Role.Sensor)
                        {

                            Console.WriteLine(client.name + " SENSOR CONNECTED");

                            addSensor(client);
                        }

                        if (client.role == Role.Instrument)
                        {
                            Console.WriteLine(client.name + " INSTRUMENT CONNECTED");

                            addInstrument(client);
                        }
                        break;
                }

            }));

        }

        int _rowSelected = 0;
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            base.RowSelected(tableView, indexPath);

            _rowSelected = indexPath.Row;

            PerformSegue("sensorSegue", this);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if(segue.Identifier == "sensorSegue")
            {
                var sensorController =
                segue.DestinationViewController as SensorViewController;

                if (sensorController != null)
                {
                    Console.WriteLine("about to segue");

                    //set selected sensor in controller
                    NSIndexPath indexPath = TableView.IndexPathForSelectedRow;
                    var sensorName = _sensorNames[indexPath.Row];
                    sensorController.sensor = _sensorDictionary[sensorName];
                }
            }

            if (segue.Identifier == "instrumentSegue")
            {
                var controller =
                segue.DestinationViewController as InstrumentViewController;

                if (controller != null)
                {
                    Console.WriteLine("about to segue");

                    //set selected sensor in controller
                    NSIndexPath indexPath = TableView.IndexPathForSelectedRow;
                    var sensorName = _sensorNames[indexPath.Row];
                    controller.sensor = _sensorDictionary[sensorName];
                }
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

