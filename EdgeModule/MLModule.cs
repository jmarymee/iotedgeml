using Microsoft.Azure.IoT.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EdgeModule
{
    public class MLModule : IGatewayModule, IGatewayModuleStart
    {
        private Broker broker;
        private string configuration;

        private Thread oThread;

        private bool quitThread = false;

        public void Create(Broker broker, byte[] configuration)
        {

            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration);

        }

        public void Destroy()
        {
            quitThread = true;
            oThread.Join();
        }

        public void Receive(Message received_message)
        {
            //Just Ignore the Message. Sensor doesn't need to print.
        }

        public void Start()
        {
            oThread = new Thread(new ThreadStart(this.threadBody));
            // Start the thread
            oThread.Start();
        }

        public void threadBody()
        {
            Random r = new Random();
            int n = r.Next();

            while (!quitThread)
            {
                Dictionary<string, string> thisIsMyProperty = new Dictionary<string, string>();
                thisIsMyProperty.Add("source", "sensor");

                Message messageToPublish = new Message("SensorData: " + n, thisIsMyProperty);

                this.broker.Publish(messageToPublish);

                //Publish a message every 5 seconds. 
                Thread.Sleep(5000);
                n = r.Next();
            }
        }
    }
}
