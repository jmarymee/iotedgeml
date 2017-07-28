using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMMLCLassLib
{
    public class Criteo
    {
        private static string modelPath = @"C:\tools\TLC\Projects\Criteo\Models\criteo-ap.zip";

        private class CriteoExample
        {
            [VectorType(13)] // Next example illustrates specifying this dimensionality at runtime
            public float[] NumFeatures;
            [VectorType(26)] // Next example illustrates specifying this dimensionality at runtime
            public string[] CatFeatures;
        }



        public static void SimplePredict()
        {
            // Initialize the environment.
            var host = new TlcEnvironment();

            // Load the pipeline and predictor from the model file.
            var predictor = host.CreatePredictionEngine<CriteoExample,
            SimplePredictionEngine.Prediction>(File.OpenRead(modelPath));

            // Produce prediction for a specific example.
            var result = predictor.Predict(new CriteoExample
            {
                NumFeatures = new[] { 1, 2, 3, 4, 5, 6, 7, 8, float.NaN, 10, 11, 12, 13 },
                CatFeatures = new[] { "a", "bxx", "c03", "0xfd", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" }
            });

            // Output the results.
            Console.WriteLine(result.Score);
            Console.WriteLine(result.Probability);
        }

    }
}
