﻿using System;
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

        public SensorData()
        {
        }
    }

    public partial class SensorListViewController : UITableViewController
    {
        List<string> _sensorNames = new List<string>();
        List<SensorData> _sensorDataList = new List<SensorData>();
        Dictionary<string, SensorData> _sensorDictionary = new Dictionary<string, SensorData>();

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

        public SensorListViewController() : base("SensorListViewController", null)
        {
        }

        public SensorListViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            TableView.Source = new SensorSource(_sensorNames, _sensorDictionary);

            Manager manager = new Manager();

            manager.connect(@"nats://192.168.86.27:4222");
            manager.init();

            //addobserver(observerex) notifies with data which is the added client in this case
            manager.addObserver(new ObserverAdapterEx((o, h, c) =>
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

                                //listen for run state changes
                                client.addObserver(new ObserverAdapterEx((anobject, ahint, data) =>
                                {
                                    Console.WriteLine((ahint as string));
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

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
