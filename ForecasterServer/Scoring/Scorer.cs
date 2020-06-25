using Accord;
using Accord.Math.Optimization.Losses;
using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using Forecaster.Server.Prediction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.Scoring
{
    public static class Scorer
    {
        public static List<double> CalculateAverageRMSE(string dirWithFiles, IEnumerable<IPredictionAlgorithm> algorithms, out double averagePredictionNum, out double averageActualPrice)
        {
            List<double> rmseValues = new List<double>();

            List<List<double>> rmseLists = InitRmseLists(algorithms.Count());

            string[] filesPath = System.IO.Directory.GetFiles(dirWithFiles);

            //IEnumerable<BasicDataset> predicted = PredictionController.Predict(filesPath[0], new MovingAverage, out IEnumerable<BasicDataset> datasets);

            int fileNum = 1;

            averagePredictionNum = 0; averageActualPrice = 0;

            foreach (string filePath in filesPath)
            {
                int alghNum = 0;

                try
                {
                    foreach (IPredictionAlgorithm algorithm in algorithms)
                    {
                        IEnumerable<BasicDataset> predicted = PredictionController.Predict(filePath, algorithm, out IEnumerable<BasicDataset> datasets);

                        datasets = datasets.OrderBy(a => a.Date);

                        averagePredictionNum += predicted.Count();

                        ++fileNum;

                        SplitSet(datasets, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet);

                        averageActualPrice += controlSet.Select(a => (double)a.Close).Average();

                        double rmse = GetRMSEAnother(controlSet, predicted);

                        rmseLists[alghNum++].Add(rmse);
                    }
                }
                catch
                {

                }
            }

            foreach (List<double> rmseList in rmseLists)
                rmseValues.Add(rmseList.Average());

            averagePredictionNum /= fileNum;
            averageActualPrice /= fileNum;

            return rmseValues;
        }

        private static double GetRMSE(IEnumerable<BasicDataset> expected, IEnumerable<BasicDataset> predicted)
        {
            double[] expectedValues = expected.Select(a => (double)a.Close).ToArray(),
                predictedValues = predicted.Select(a => (double)a.Close).ToArray();

            var squareLoss = new SquareLoss(expectedValues)
            {
                Mean = false,
                Root = true
            };

            var rmse = squareLoss.Loss(predictedValues);

            return rmse;
        }

        private static double GetRMSEAnother(IEnumerable<BasicDataset> expected, IEnumerable<BasicDataset> predicted)
        {
            double[] expectedValues = expected.Select(a => (double)a.Close).ToArray(),
                predictedValues = predicted.Select(a => (double)a.Close).ToArray();

            double[] diff = new double[expectedValues.Length], powed = new double[expectedValues.Length];

            for (int i = 0; i < expectedValues.Length; ++i) {
                diff[i] = predictedValues[i] - expectedValues[i];
                powed[i] = Math.Pow(diff[i], 2);
            }

            double rmse = Math.Sqrt(powed.Average());

            return rmse;
        }

        private static void SplitSet(IEnumerable<BasicDataset> datasets, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet)
        {
            int datasetCount = datasets.Count(),
                trainingSize = (int)Math.Ceiling(datasetCount * 0.8),
                controlSize = datasetCount - trainingSize;

            trainingSet = datasets.Take(trainingSize);

            controlSet = datasets.Skip(trainingSize).Take(controlSize);
        }

        private static List<List<double>> InitRmseLists(int count)
        {
            List<List<double>> rmseLists = new List<List<double>>(count);

            for (int i = 0; i < rmseLists.Capacity; ++i)
                rmseLists.Add(new List<double>());

            return rmseLists;
        }
    }
}
