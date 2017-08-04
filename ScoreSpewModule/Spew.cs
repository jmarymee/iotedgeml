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

        public class ScoreSpewData
        {
            [JsonProperty(PropertyName = "sensor1")]
            public double sensor1 { get; set; }

            [JsonProperty(PropertyName = "sensor2")]
            public double sensor2 { get; set; }
        }

        public class ScoreSpewConfig
        {
            [JsonProperty(PropertyName = "pathToTelemetry")]
            public string pathToTelemetry { get; set; }
        }

        [JsonArray]
        public class PredictArray
        {
            List<Double> scoreData = new List<double>() { 0, 1, 2, 3, 4, 9, 0, 5, 2 };
        }

        public void Create(Broker broker, byte[] configuration)
        {
            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration, 0, configuration.Length);
            Console.WriteLine(this.configuration);

            try
            {
                dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(this.configuration);
                string myPath = myConfig.pathToTelemetry;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

        }

        public void Start()
        {
            ScoreSpewConfig sc = new ScoreSpewConfig() { pathToTelemetry = ".\telemetry.csv" };
            string js = JsonConvert.SerializeObject(sc);
            string configuration = "{\"pathToTelemetry\":\".\telemetry.csv\"}";
            dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(configuration);
            string myPath = myConfig.pathToTelemetry;

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
                //Console.WriteLine(jsonData);
                this.broker.Publish(messageToPublish);
                //Testing
                //Console.WriteLine("I am sending the message from Spew");

                //This is for the ML predictor
                //List<Double> scoreData = new List<double>() { 0, 1, 2, 3, 4, 9, 0, 5, 2 };
                float[] scoreData = new float[] { 0, 1, 2, 3, 4, 9, 0, 5, 2 };
                PredictArray pa = new PredictArray();
                string mlPredictString = JsonConvert.SerializeObject(scoreData);
                Dictionary<string, string> thisIsMyPropertyML = new Dictionary<string, string>();
                thisIsMyPropertyML.Add("source", "predict");
                Message messageToPublishML = new Message(mlPredictString, thisIsMyPropertyML);
                Console.WriteLine(mlPredictString);
                this.broker.Publish(messageToPublishML);

                //Publish a message every 5 seconds. 
                Thread.Sleep(10000);
                sd = new ScoreSpewData() { sensor1 = r.NextDouble(), sensor2 = r.NextDouble() };
                jsonData = JsonConvert.SerializeObject(sd);
            }
        }
    }
}
