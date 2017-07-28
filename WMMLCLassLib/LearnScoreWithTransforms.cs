//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks/*;*/
////namespace WMMLCLassLib

//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using Microsoft.MachineLearning.Learners;
using static Microsoft.MachineLearning.Data.MetadataUtils;

namespace WMMLCLassLib
{
    /// <summary>
    /// This examples demonstrates running the training programmatically, when the data resides in memory.
    /// 
    /// The training data for this example is the 'Criteo Kaggle challenge' dataset, which has following columns:
    ///   Label: float
    ///   NumFeatures: 13 floats
    ///   CatFeatures: 26 strings
    /// </summary>
    public static class TrainFromMemory
    {
        /// <summary>
        /// This class describes the training examples. 
        /// </summary>
        public class CriteoExample
        {
            public float Label;

            // The features column. It is important to specify dimensionality of the features, otherwise
            // it will be assumed 'variable-length' and the normalizer transform will complain.
            [VectorType(13)]
            public float[] NumFeatures;

            // The categorical column. Here, specifying the dimensionality is not critical: even if the data is assumed to
            // be variable length, the hash-based encoding supports this and produces a fixed-size vector.
            [VectorType(26)]
            public string[] CatFeatures;
        }

        public class ScoredData
        {
            public float Score;
        }

        public static void Execute()
        {
            // Obtain the examples in memory somehow. For the demonstration purposes, data is loaded from disk.
            List<CriteoExample> examples = LoadExamples().ToList();

            // Initialize the environment.
            using (var env = new TlcEnvironment())
            {
                // Create a data view of the training data.
                var trainingData = env.CreateDataView(examples);

                // Append trasformations.
                // 1. Normalize numeric features.
                var norm = env.CreateTransform("ZScore{col=NumFeatures}", trainingData);

                // 2. Append missing indicator to numeric features.
                var missing = env.CreateTransform("NAHandle{col=NumFeatures}", norm);

                // 3. Hash-based one-hot encoding into 20-bit space for categorical features.
                var catHash = env.CreateTransform("CatHash{col=CatFeatures bits=20}", missing);

                // 4. Form the joint feature vector.
                var concat = env.CreateTransform("Concat{col=Features:NumFeatures,CatFeatures}", catHash);

                // Train a predictor.
                var trainingExamples = env.CreateExamples(concat, "Features", "Label");
                var predictor = env.TrainPredictor(new LogisticRegression.Arguments { optTol = 0.01f }, trainingExamples);

                // Save to the model file.
                using (var memoryStream = File.Create(ModelFilePath))
                    env.SaveModel(memoryStream, predictor, trainingExamples);

                // Save the model as code
                using (var codeStream = File.Create(ModelCodeFilePath))
                {
                    //env.SaveModelAsCode(codeStream, predictor, trainingExamples.Schema);
                    env.SaveModelAsCode(codeStream, predictor, null);
                }

                // Run prediction programmatically.
                var scorer = env.CreateDefaultScorer(trainingExamples, predictor);
                var predictionEngine = env.CreatePredictionEngine<CriteoExample, ScoredData>(scorer);
                var result = predictionEngine.Predict(examples[7]);
                Console.WriteLine(result.Score);
            }
        }

        private static IEnumerable<CriteoExample> LoadExamples()
        {
            return File.ReadLines(DataFilePath)
                .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#") && !line.StartsWith("Label"))
                .Select(
                    line =>
                    {
                        var parts = line.Split('\t');
                        return
                            new CriteoExample
                            {
                                Label = int.Parse(parts[0]),
                                NumFeatures = parts.Skip(1).Take(13).Select(x => x == "" ? float.NaN : float.Parse(x, CultureInfo.InvariantCulture)).ToArray(),
                                CatFeatures = parts.Skip(14).ToArray()
                            };
                    });
        }

        private static string DataFilePath { get { return Path.Combine(@"C:\tools\TLC\Projects\Criteo\Data", @"criteo-tiny.tsv"); } }
        private static string ModelFilePath { get { return Path.Combine(@"C:\tools\TLC\Projects\Criteo\Models", "model.zip"); } }
        private static string ModelCodeFilePath { get { return Path.Combine(@"C:\tools\TLC\Projects\Criteo", "model-code.txt"); } }
    }
}


