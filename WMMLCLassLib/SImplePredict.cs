using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.MachineLearning;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.CommandLine;
using Microsoft.MachineLearning.Data;
using static Microsoft.MachineLearning.Data.MetadataUtils;

namespace WMMLCLassLib
{
    /// <summary>
    /// This demonstrates the simple scenario of API use: run trained model on an already featurized example.
    /// In our case, the model is trained on the 'breast cancer' dataset, it's a binary classifier, and the input 
    /// data is an array of 9 features.
    /// 
    /// In this case, we can use the <see cref="SimplePredictionEngine"/>. It is feasible whenever:
    ///   * The input data is an array of floating-point values of known size.
    ///   * The predictor is either a binary classifier or a regressor.
    /// </summary>
    public static class SimplePredict
    {
        public static void Execute()
        {
            // Initialize the API environment.
            using (var env = new TlcEnvironment())
            {
                Setup(env);

                // Load the prediction engine from the model file. 
                using (var fs = File.OpenRead(ModelFilePath))
                {
                    var predictionEngine = env.CreateSimplePredictionEngine(fs, 9);

                    // Get prediction
                    var prediction = predictionEngine.Predict(new float[] { 0, 1, 2, 3, 4, 9, 0, 5, 2 });

                    // Output results
                    Console.WriteLine(prediction.Score);
                    Console.WriteLine(prediction.Probability);
                }
            }
        }

        /// <summary>
        /// The setup necessary to run the sample. In this case we're training the model.
        /// </summary>
        private static void Setup(IHostEnvironment env)
        {
            //string src = "";
            //new TrainCommand(env,
            //    new TrainCommand.Arguments
            //    {
            //        dataFile = Path.Combine(src, @"UCI\breast-cancer.txt"),
            //        trainer = new SubComponent<ITrainer, SignatureTrainer>("AP"),
            //        outputModelFile = ModelFilePath
            //    }).Run();

            var args = new TrainCommand.Arguments();
            args.dataFile = @"C:\tools\TLC\Samples\Data\UCI\breast-cancer.txt";
            args.trainer = new SubComponent<ITrainer, SignatureTrainer>("AP");
            args.outputModelFile = ModelFilePath;

            new TrainCommand((TrainCommand.Arguments)args, env).Run();
        }

        private static string ModelFilePath { get { return @"C:\tools\TLC\Projects\model.zip"; } }
    }
}
