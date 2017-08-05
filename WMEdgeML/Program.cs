using ScoreSpewModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMEdgeML
{
    class Program
    {
        static void Main(string[] args)
        {
            //WMMLCLassLib.MLClass mlClass = new WMMLCLassLib.MLClass();
            //mlClass.ScoreLoadedModel();

            //Works for simple float arrays
            //WMMLCLassLib.SimplePredict.Execute();

            //This is more complex
            //WMMLCLassLib.Criteo.SimplePredict();
            //WMMLCLassLib.Criteo2.SimplePredict();
            //WMMLCLassLib.TrainFromMemory.Execute();

            //WMMLCLassLib.MLAnomolyGenerator a = new WMMLCLassLib.MLAnomolyGenerator();
            //var data = a.GenerateDeviceData(10, 1, 50);

            //Spew spew = new Spew();
            //spew.Start();

            float[] vals = ReadFirstNumerical(@"C:\repos\TLCProjects\Data\test.csv");

            //Test failure prediction
            WMMLCLassLib.FailurePrediction fp = new WMMLCLassLib.FailurePrediction(@"C:\repos\TLCProjects\Models\0.model.zip");
            WMMLCLassLib.FailurePrediction.PredictionValues pv = fp.Predict(vals);

            Console.WriteLine("Any Key to stop");
            Console.ReadLine();
            //spew.Destroy();

        }

        public static float[] ReadFirstNumerical(string pathToFile)
        {
            float[] vals = null;
            bool isFirst = true;
            string line;

            using (System.IO.StreamReader file = new StreamReader(pathToFile))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        vals = Array.ConvertAll(line.Split(','), float.Parse);
                        break;
                    }
                }
            }
            return vals;
        }
    }
}
