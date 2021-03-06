﻿using Microsoft.Azure.IoT.Gateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EdgeModule
{
    public class MLModule : IGatewayModule//, IGatewayModuleStart
    {
        private Broker broker;
        private string configuration;
        private string modelPath;
        private WMMLCLassLib.FailurePrediction fp;
        private float failureThreshold = 0.0f;

        private Thread oThread;

        private bool quitThread = false;
        private bool isLog = false;

        public class FailureNotice
        {
            [JsonProperty(PropertyName = "deviceid")]
            public int deviceID { get; set; }

            [JsonProperty(PropertyName = "failscore")]
            public double failScore { get; set; }

            [JsonProperty(PropertyName = "probability")]
            public double probability { get; set; }
        }

        public void Create(Broker broker, byte[] configuration)
        {

            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration);

            try
            {
                dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(this.configuration);
                modelPath = myConfig.pathToModel;
                failureThreshold = myConfig.failureThreshold;
                string log = myConfig.log;
                if (log.Equals("true"))
                {
                    isLog = true;
                }
                if (!File.Exists(modelPath))
                {
                    throw new FileNotFoundException("Model file does not exist");
                }
                else
                {
                    fp = new WMMLCLassLib.FailurePrediction(modelPath);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            //Attempt assembly load
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("CANNOT LOAD!");
            throw new NotImplementedException();
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
            //if (received_message.Properties["source"] == "sensor")
            //{
            //    string jsonString = Encoding.UTF8.GetString(received_message.Content, 0, received_message.Content.Length);
            //    Console.WriteLine("I am in the ML Module");
            //    Console.WriteLine(jsonString);
            //    ScoreSpewData sd = JsonConvert.DeserializeObject<ScoreSpewData>(jsonString);
            //}
            if (received_message.Properties["source"] == "predict")
            {
                if (fp == null)
                {
                    Console.WriteLine("Scorer not loaded. Exiting...");
                    return;
                }
                string jsonString = Encoding.UTF8.GetString(received_message.Content, 0, received_message.Content.Length);
                //List<Double> predict = JsonConvert.DeserializeObject<List<double>>(jsonString);
                float[] predict = JsonConvert.DeserializeObject<float[]>(jsonString);
                //string pathToModel = @"C:\Users\jmarymee\Documents\Visual Studio 2017\Projects\iotedgeml\ScoreSpewModule\model.zip";
                //WMMLCLassLib.SimplePredict.Predict(modelPath, predict);
                WMMLCLassLib.FailurePrediction.PredictionValues predictionValues = fp.Predict(predict);
                if (isLog) Console.WriteLine(String.Format("Score: {0}, Threshold: {1}, Probabilities: {2}", predictionValues.Score, failureThreshold, predictionValues.Probability));
                if (predictionValues.Score < failureThreshold)
                {
                    if (isLog) Console.WriteLine("FAILURE IMMINENT!");
                    PublishAnomoly(predictionValues);
                }
                else
                {
                    if (isLog) Console.WriteLine("Scored device within tolerances");
                }
            }
        }

        public bool PublishAnomoly(WMMLCLassLib.FailurePrediction.PredictionValues pv)
        {
            Dictionary<string, string> thisIsMyProperty = new Dictionary<string, string>();
            thisIsMyProperty.Add("source", "predictionmodule");

            FailureNotice fn = new FailureNotice() { deviceID = 1, failScore = pv.Score, probability = pv.Probability };
            string message = JsonConvert.SerializeObject(fn);

            Message messageToPublish = new Message(message, thisIsMyProperty);

            this.broker.Publish(messageToPublish);

            if (isLog) Console.WriteLine("Published Anomaly to Broker");

            return true;
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
