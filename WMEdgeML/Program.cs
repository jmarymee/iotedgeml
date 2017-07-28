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

            WMMLCLassLib.SimplePredict.Execute();
        }
    }
}
