using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace ecomms_ios
{
    public class SensorData
    {
        public String name { get; set; }
        public String temperature { get; set; }
        public String description { get; set; }
        public String location { get; set; }

        public SensorData()
        {
        }
    }

    public partial class SensorListViewController : UITableViewController
    {
        List<string> _sensorNames = new List<string>();
        List<SensorData> _sensorDataList = new List<SensorData>();
        Dictionary<string, SensorData> _sensorDictionary = new Dictionary<string, SensorData>();

        UIRefreshControl _refreshControl = new UIRefreshControl();

        //ECOMMS Manager
        Manager _manager;

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
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //base.RowSelected(tableView, indexPath);

                Console.WriteLine("ROW SELECTED {0}", indexPath.Row);
            }
        }

        public SensorListViewController() : base("SensorListViewController", null)
        {
        }

        public SensorListViewController(IntPtr handle) : base(handle)
        {
        }

        private void addSensor(IClient client)
        {
            if (client.role == Role.Sensor)
            {

                Console.WriteLine(client.name + " SENSOR ADDED");

                if (!_sensorNames.Contains(client.name))
                {
                    _sensorNames.Add(client.name);
                    _sensorDataList.Add(new SensorData());
                    _sensorDictionary.Add(client.name, new SensorData());

                    //get the location
                    client.doGet("location", (response) =>
                    {
                        _sensorDictionary[client.name].location = response;
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
                                _sensorNames.Remove(me.name);

                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    TableView.ReloadData();
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
                            Encoding.UTF8.GetString(bytes, 0, bytes.Length));

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
            TableView.Delegate = new SensorDelegate();

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

            _manager.connect(@"nats://192.168.86.27:4222");
            _manager.init();

            //addobserver(observerex) notifies with data which is the added client in this case
            _manager.addObserver(new ObserverAdapterEx((o, h, c) =>
            {
                //need to wait to notify until after base class has gotton response
                //to role request
                //or have library query first before creating client
                //WIP...

                var client = c as IClient;
                Thread.Sleep(3000);
                switch (h)
                {
                    case "CONNECTED":

                        if (client.role == Role.Sensor)
                        {

                            Console.WriteLine(client.name + " SENSOR CONNECTED");

                            if(!_sensorNames.Contains(client.name))
                            {
                                _sensorNames.Add(client.name);
                                _sensorDataList.Add(new SensorData());
                                _sensorDictionary.Add(client.name, new SensorData());

                                //get the location
                                client.doGet("location", (response) =>
                                {
                                    _sensorDictionary[client.name].location = response;
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

                                    if(hint.Equals("ONLINE_CHANGED"))
                                    {
                                        IClient me = observable as IClient;

                                        if(!me.online)
                                        {
                                            _sensorNames.Remove(me.name);

                                            MainThread.BeginInvokeOnMainThread(() =>
                                            {
                                                TableView.ReloadData();
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
                                        Encoding.UTF8.GetString(bytes, 0, bytes.Length));

                                    _sensorDictionary[client.name].description = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                                    _sensorDictionary[client.name].name = client.name;

                                    MainThread.BeginInvokeOnMainThread(() =>
                                    {
                                        TableView.ReloadData();
                                    });
                                });
                            }
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
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

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

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

