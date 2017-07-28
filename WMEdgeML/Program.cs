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
            //WMMLCLassLib.SimplePredict.Execute();

            //This is more complex
            //WMMLCLassLib.Criteo.SimplePredict();
            WMMLCLassLib.Criteo2.SimplePredict();
          
        }
    }
}
