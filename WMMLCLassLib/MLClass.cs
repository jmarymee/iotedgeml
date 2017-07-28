using Microsoft.MachineLearning.Data;
using Microsoft.MachineLearning.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WMMLCLassLib
{
    public class MLClass
    {
        public void ScoreLoadedModel()
        {
            //Initialize TLC
            var host = new TlcEnvironment();

            // Load the pipeline and predictor from the model file. 9 is #features. 
            // Note that CreateSimplePredictionEngine is an extension method defined in Microsoft.MachineLearning.Api
            var predictor = host.CreateSimplePredictionEngine(File.OpenRead(@"C:\tools\TLC\Projects\AIC\Models\4.model.zip"), 9);
            //var predictor = host.CreateSimplePredictionEngine(File.OpenRead(@"C:\tools\TLC\Projects\BreastCancer\Models\0.model.zip"), 9);

            // Produce prediction.
            var result = predictor.Predict(new float[] { 8, 10, 10, 8, 7, 10, 9, 7, 1 });

            // Output the results.
            Console.WriteLine(result.Score);
            Console.WriteLine(result.Probability);


        }
    }
}
