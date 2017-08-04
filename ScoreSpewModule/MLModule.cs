using Microsoft.Azure.IoT.Gateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreSpewModule
{
    public class MLModule : IGatewayModule//, IGatewayModuleStart
    {
        private Broker broker;
        private string configuration;

        private Thread oThread;

        private bool quitThread = false;

        public class ScoreSpewData
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

        public void Destroy()
        {
            quitThread = true;
            if (oThread != null)
            {
                oThread.Join();
            }
        }

        public void Receive(Message received_message)
        {
            if (received_message.Properties["source"] == "sensor")
            {
                string jsonString = Encoding.UTF8.GetString(received_message.Content, 0, received_message.Content.Length);
                Console.WriteLine("I am in the ML Module");
                Console.WriteLine(jsonString);
                ScoreSpewData sd = JsonConvert.DeserializeObject<ScoreSpewData>(jsonString);
            }
            if (received_message.Properties["source"] == "predict")
            {
                string jsonString = Encoding.UTF8.GetString(received_message.Content, 0, received_message.Content.Length);
                //List<Double> predict = JsonConvert.DeserializeObject<List<double>>(jsonString);
                float[] predict = JsonConvert.DeserializeObject<float[]>(jsonString);
                string pathToModel = @"C:\Users\jmarymee\Documents\Visual Studio 2017\Projects\iotedgeml\ScoreSpewModule\model.zip";
                WMMLCLassLib.SimplePredict.Predict(pathToModel, predict);
            }
        }

        //public void Start()
        //{
        //    oThread = new Thread(new ThreadStart(this.threadBody));
        //    // Start the thread
        //    oThread.Start();
        //}

        public void threadBody()
        {
            //Random r = new Random();
            //int n = r.Next();

            //while (!quitThread)
            //{
            //    Dictionary<string, string> thisIsMyProperty = new Dictionary<string, string>();
            //    thisIsMyProperty.Add("source", "sensor");

            //    Message messageToPublish = new Message("SensorData: " + n, thisIsMyProperty);

            //    this.broker.Publish(messageToPublish);

            //    //Publish a message every 5 seconds. 
            //    Thread.Sleep(5000);
            //    n = r.Next();
            //}
        }
    }
}
