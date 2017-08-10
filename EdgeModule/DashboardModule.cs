﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.IoT.Gateway;

namespace EdgeModule
{
    class DashboardModule : IGatewayModule
    {
        private Broker broker;
        private string configuration;
        private bool isLog = false; //Logging on?

        private RIUtilscs rUtils;

        public void Create(Broker broker, byte[] configuration)
        {

            this.broker = broker;
            this.configuration = Encoding.UTF8.GetString(configuration);

            rUtils = new RIUtilscs(); // "C:\tools\Newton\Newtonsoft.Json.dll");

            try
            {
                dynamic myConfig = rUtils.GetConfigObject(this.configuration);
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
            if (received_message.Properties["name"] == "predictmodule")
            {
                string jsonString = Encoding.UTF8.GetString(received_message.Content, 0, received_message.Content.Length);
                dynamic myConfig = rUtils.JsonDeserializeFailure(jsonString); //Newtonsoft.Json.Linq.JObject.Parse(jsonString);

                if (isLog) Console.WriteLine(String.Format("Failing device ID: {0} with a Score of {1} and a Probability of {2}", myConfig.deviceID, myConfig.failScore, myConfig.probability));
            }
        }
    }
}
