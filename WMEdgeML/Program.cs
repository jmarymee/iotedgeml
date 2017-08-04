using ScoreSpewModule;
using System;
using System.Collections.Generic;
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
            WMMLCLassLib.SimplePredict.Execute();

            //This is more complex
            //WMMLCLassLib.Criteo.SimplePredict();
            //WMMLCLassLib.Criteo2.SimplePredict();
            //WMMLCLassLib.TrainFromMemory.Execute();

            //WMMLCLassLib.MLAnomolyGenerator a = new WMMLCLassLib.MLAnomolyGenerator();
            //var data = a.GenerateDeviceData(10, 1, 50);

            //Spew spew = new Spew();
            //spew.Start();

            Console.WriteLine("Any Key to stop");
            Console.ReadLine();
            //spew.Destroy();
          
        }
    }
}
