using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.IoT.Gateway;
using Newtonsoft.Json;

namespace EdgeModule
{
    class DashboardModule : IGatewayModule
    {
        private Broker broker;
        private string configuration;
        private bool isLog = false; //Logging on?
        public void Create(Broker broker, byte[] configuration)
        {

            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration);

            try
            {
                dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(this.configuration);
                string log = myConfig.log;
                if (log.Equals("true"))
                {
                    isLog = true;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

        }

        public void Destroy()
        {
        }

        public void Receive(Message received_message)
        {
            if (received_message.Properties["source"] == "predictionmodule")
            {
                string jsonString = Encoding.UTF8.GetString(received_message.Content, 0, received_message.Content.Length);
                dynamic myConfig = Newtonsoft.Json.Linq.JObject.Parse(jsonString);

                if (isLog) Console.WriteLine(String.Format("Failing device ID: {0} with a Score of {1} and a Probability of {2}", myConfig.deviceid, myConfig.failscore, myConfig.probability));
            }
        }
    }
}
