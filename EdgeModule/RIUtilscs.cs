using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;


namespace EdgeModule
{
    public class RIUtilscs
    {
        //used for late binding and type reflection of the Newtonsoft library
        private Assembly JsonAssembly;
        private Type JsonType;
        private Object JsonObject;
        private MethodInfo[] JsonStaticMethods;
        private MethodInfo jParseMethod;
        Type jsonConvertType;
        MethodInfo[] methods;
        MethodInfo deserialize;
        MethodInfo serialize;

        private string assemblyPath;

        string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        public RIUtilscs()
        {
            Configuration config = null;
            string exeConfigPath = this.GetType().Assembly.Location;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch (Exception ex)
            {
                //handle errror here.. means DLL has no sattelite configuration file.
            }

            if (config != null)
            {
                string assemblyPath = GetAppSetting(config, "newtonassembly");
                //assemblyPath = ConfigurationManager.AppSettings["newtonassembly"];
                InitJSONLateBinding(assemblyPath);
            }

        }

        public dynamic GetConfigObject(string jsonConfigString)
        {
            dynamic myConfig = jParseMethod.Invoke(null, new object[] { jsonConfigString });
            return myConfig;
        }

        public float[] JsonDeserializeFloatArray(string jsonString)
        {
            Type listType = typeof(float[]);

            float[] data = (float[])deserialize.Invoke(null, new object[] { jsonString, listType });

            return data;
        }

        public dynamic JsonDeserializeFailure(string jsonString)
        {
            Type failType = typeof(FailureNotice);
            object o = deserialize.Invoke(null, new object[] { jsonString, failType });

            return (dynamic)o;
        }

        public string serializeFloat(float[] data)
        {
            return (string)this.serialize.Invoke(null, new object[] { data });
        }

        public string serializeFailure(FailureNotice f)
        {
            object data = this.serialize.Invoke(null, new object[] { f });

            return (string)data;
        }

        public void InitJSONLateBinding(string assemblyName)
        {

            //Loads the assembly
            JsonAssembly = Assembly.LoadFile(assemblyName);

            JsonType = JsonAssembly.GetType("Newtonsoft.Json.Linq.JObject");
            JsonObject = Activator.CreateInstance(JsonType);
            JsonStaticMethods = JsonType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            jParseMethod = JsonStaticMethods[4];

            jsonConvertType = JsonAssembly.GetType("Newtonsoft.Json.JsonConvert");
            methods = jsonConvertType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            deserialize = methods[37];

            serialize = methods[27];
        }
    }

    public class FailureNotice
    {
        //[JsonProperty(PropertyName = "deviceid")]
        public int deviceID { get; set; }

        //[JsonProperty(PropertyName = "failscore")]
        public double failScore { get; set; }

        //[JsonProperty(PropertyName = "probability")]
        public double probability { get; set; }
    }
}
