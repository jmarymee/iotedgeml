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
    public class FailurePrediction
    {
        public string pathToTrainedModel { get; set; }

        private TlcEnvironment tlcEnv = null;
        private PredictionEngine<FailurePredict, SimplePredictionEngine.Prediction> spe = null;

        private class FailurePredict
        {
            [VectorType(1)]
            public float[] Yield_Pass_Fail;

            [VectorType(50)]
            public float[] Features;

            [ColumnNameAttribute("Label")]
            public float Label;
        }

        public FailurePrediction(string pathToModel)
        {
            if (!File.Exists(pathToModel)) { return; }

            tlcEnv = new TlcEnvironment();
            using (var fs = File.OpenRead(pathToModel))
            {
                //spe = tlcEnv.CreateSimplePredictionEngine(fs, 51);
                spe = tlcEnv.CreatePredictionEngine<FailurePredict, SimplePredictionEngine.Prediction>(fs);
            }
        }

        public PredictionValues Predict(float[] scoreData)
        {
            FailurePredict fp = new FailurePredict();
            fp.Yield_Pass_Fail = new float[1];
            fp.Yield_Pass_Fail[0] = scoreData[0];
            fp.Features = new float[50];
            ArrayCopy(scoreData, fp.Features, 50, 1);
            var prediction = spe.Predict(fp);
            return new PredictionValues(prediction.Score, prediction.Probability);
        }

        private void ArrayCopy(float[] source, float[] dest, int length, int start)
        {
            for(int loop=0; loop<length; loop++)
            {
                dest[loop] = source[loop + 1];
            }
        }

        public class PredictionValues
        {
            public PredictionValues(float score, float probability)
            {
                m_score = score;
                m_prob = probability;
            }
            private float m_score;
            public float Score
            {
                get { return m_score; }
            }

            private float m_prob;
            public float Probability
            {
                get { return m_prob; }
            }
        }
    }
}
