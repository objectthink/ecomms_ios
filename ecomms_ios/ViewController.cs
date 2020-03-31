using Foundation;
using System;
using UIKit;
using ECOMMS_Manager;
using ECOMMS_Entity;
using ECOMMS_Client;
using System.Threading;
using System.Text;

namespace ecomms_ios
{
    public partial class ViewController : UIViewController
    {
        
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            Manager manager = new Manager();

            manager.connect(@"nats://192.168.86.20:4222");
            manager.init();

            //addobserver(observerex) notifies with data which is the added client in this case
            manager.addObserver(new ObserverAdapterEx((o, h, c) =>
            {
                //need to wait to notify until after base class has gotton response
                //to role request
                //or have library query first before creating client
                //WIP...

                var client = c as IClient;
                Thread.Sleep(1000);
                switch (h)
                {
                    case "CONNECTED":

                        if (client.role == Role.Sensor)
                        {

                            Console.WriteLine(client.name + " SENSOR CONNECTED");

                            //listen for run state changes
                            client.addObserver(new ObserverAdapterEx((anobject, ahint, data) =>
                            {
                                Console.WriteLine((ahint as string));
                            }));

                            //add a status listener
                            client.addStatusListener((name, bytes) =>
                            {
                                Console.WriteLine("{0}:status listener:{1}:{2}", client.name, name, Encoding.UTF8.GetString(bytes, 0, bytes.Length));

                                _label.BeginInvokeOnMainThread(()=> {
                                    _label.Text = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                                });                                
                            });
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

        partial void onClickButton(UIButton sender)
        {
            Console.WriteLine("button clicked!");

            _label.Text = "hello cruel world";
        }
    }
}