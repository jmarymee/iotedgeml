using Microsoft.Azure.IoT.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;

namespace ScoreSpewModule
{
    public class Spew : IGatewayModule, IGatewayModuleStart
    {
        private Broker broker;
        private string configuration;

        private Thread oThread;

        private bool quitThread = false;

        private List<float[]> m_spewTestData;

        private string m_pathToTelemetry;

        //public class ScoreSpewData
        //{
        //    [JsonProperty(PropertyName = "sensor1")]
        //    public double sensor1 { get; set; }

        //    [JsonProperty(PropertyName = "sensor2")]
        //    public double sensor2 { get; set; }
        //}

        //public class ScoreSpewConfig
        //{
        //    [JsonProperty(PropertyName = "pathToTelemetry")]
        //    public string pathToTelemetry { get; set; }
        //}

        //[JsonArray]
        //public class PredictArray
        //{
        //    List<Double> scoreData = new List<double>() { 0, 1, 2, 3, 4, 9, 0, 5, 2 };
        //}

        public void Create(Broker broker, byte[] configuration)
        {
            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration, 0, configuration.Length);
            Console.WriteLine(this.configuration);

            try
            {
                dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(this.configuration);
                m_pathToTelemetry = myConfig.pathToTelemetry;
                //List<float[]> entries = GetTestScoreData(myPath);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

        }

        public void Start()
        {
            //ScoreSpewConfig sc = new ScoreSpewConfig() { pathToTelemetry = ".\telemetry.csv" };
            //string js = JsonConvert.SerializeObject(sc);
            //string configuration = "{\"pathToTelemetry\":\".\telemetry.csv\"}";
            //dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(configuration);
            //m_pathToTelemetry = myConfig.pathToTelemetry;

            //string appPath = AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.PathSeparator + "iotedgml";
            var projectPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + System.IO.Path.DirectorySeparatorChar + "iotedgml";
            //var projectPath2 = System.IO.Path.GetDirectoryName(
            //    System.IO.Path.GetDirectoryName(
            //        System.IO.Path.GetDirectoryName(
            //            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase))) 

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

            //ScoreSpewData sd = new ScoreSpewData() { sensor1 = r.NextDouble(), sensor2 = r.NextDouble() };
            //string jsonData = JsonConvert.SerializeObject(sd);

            while (!quitThread)
            {
                //Dictionary<string, string> thisIsMyProperty = new Dictionary<string, string>();
                //thisIsMyProperty.Add("source", "sensor");
                //Message messageToPublish = new Message(jsonData, thisIsMyProperty);
                ////Console.WriteLine(jsonData);
                //this.broker.Publish(messageToPublish);

                //This is for the ML predictor
                //List<Double> scoreData = new List<double>() { 0, 1, 2, 3, 4, 9, 0, 5, 2 };
                //float[] scoreData = new float[] { 0, 1, 2, 3, 4, 9, 0, 5, 2 };
                float[] scoreData = null; //Each iteration var

                if (m_spewTestData == null || m_spewTestData.Count < 1)
                {
                    m_spewTestData = GetTestScoreData(m_pathToTelemetry);
                }
                else
                {
                    scoreData = m_spewTestData.FirstOrDefault<float[]>();
                    string mlPredictString = JsonConvert.SerializeObject(scoreData);
                    Dictionary<string, string> thisIsMyPropertyML = new Dictionary<string, string>();
                    thisIsMyPropertyML.Add("source", "predict");
                    Message messageToPublishML = new Message(mlPredictString, thisIsMyPropertyML);
                    Console.WriteLine(mlPredictString);
                    this.broker.Publish(messageToPublishML);
                    m_spewTestData.Remove(scoreData);
                }
                //Publish a message every 5 seconds. 
                Thread.Sleep(5000);

                //sd = new ScoreSpewData() { sensor1 = r.NextDouble(), sensor2 = r.NextDouble() };
                //jsonData = JsonConvert.SerializeObject(sd);
            }
        }

        /// <summary>
        /// This method reads in a list of prediction values that will be published to the bus.
        /// It will read the file into an array, delete the file and then feed one prediction per sleep interation until the array/list is at zero
        /// Once that happens it will then look for another file to parse in. Wash, rinse, repeat...
        /// </summary>
        /// <param name="pathToDataFile"></param>
        /// <returns></returns>
        public List<float[]> GetTestScoreData(string pathToDataFile)
        {
            if (!File.Exists(pathToDataFile)) //In this case we will return a List<float[]> with no entries and the spewer will spew nothing then sleep until the next check.
            {
                Console.WriteLine("No file found to spew!");
                return new List<float[]>();
            }
            else
            {
                Console.WriteLine("Got us a file to spew!");
            }
            string line;
            float[] vals = null;
            List<float[]> Entries = new List<float[]>();

            using (System.IO.StreamReader file = new StreamReader(pathToDataFile))
            {
                while ((line = file.ReadLine()) != null)
                {
                    vals = Array.ConvertAll(line.Split(','), float.Parse);
                    Entries.Add(vals);
                }
            }

            //Now cleanup the spew test file
            File.Delete(pathToDataFile);

            return Entries;
        }
    }
}
