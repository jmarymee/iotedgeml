using Microsoft.Azure.IoT.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
//using EdgeModule;

namespace ScoreSpewModule
{
    public class Spew : IGatewayModule, IGatewayModuleStart
    {
        private Broker broker;
        private string configuration;

        private Thread oThread;

        private bool quitThread = false;

        //private MLModule mlm;

        public  class ScoreSpewData
        {
            [JsonProperty(PropertyName = "sensor1")]
            public double sensor1 { get; set; }

            [JsonProperty(PropertyName = "sensor2")]
            public double sensor2 { get; set; }
        }

        public void Create(Broker broker, byte[] configuration)
        {

            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration);

        }

        public void Start()
        {
            //mlm = new MLModule(); //used for tight binding test

            oThread = new Thread(new ThreadStart(this.threadBody));
            // Start the thread
            oThread.Start();
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

        public void threadBody()
        {
            Random r = new Random();
            //int n = r.Next();
            double d = r.NextDouble();

            ScoreSpewData sd = new ScoreSpewData() { sensor1 = r.NextDouble(), sensor2 = r.NextDouble() };
            string jsonData = JsonConvert.SerializeObject(sd);

            while (!quitThread)
            {
                Dictionary<string, string> thisIsMyProperty = new Dictionary<string, string>();
                thisIsMyProperty.Add("source", "sensor");

                Message messageToPublish = new Message(jsonData, thisIsMyProperty);
                Console.WriteLine(jsonData);

                this.broker.Publish(messageToPublish);
                //Testing
                Console.WriteLine("I am sending the message from Spew");

                //Publish a message every 5 seconds. 
                Thread.Sleep(3000);
                sd = new ScoreSpewData() { sensor1 = r.NextDouble(), sensor2 = r.NextDouble() };
                jsonData = JsonConvert.SerializeObject(sd);
            }
        }
    }
}
