using Extreme.DataAnalysis;
using Extreme.Mathematics;
using Extreme.Statistics.TimeSeriesAnalysis;
using Forecaster.DataHandlers;
using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Prediction
{
    public class AutoArima : BasicAlgorithm, IPredictionAlgorithm
    {
        public IEnumerable<BasicDataset> Predict(IEnumerable<BasicDataset> dataset)
        {
            CheckIncomingSet(dataset);

            DatasetHandler.SplitSet(dataset, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet);

            int trainingCount = trainingSet.Count(), controlCount = controlSet.Count();

            DateTime[] predictedDates = new DateTime[controlCount];

            for (int i = 0; i < predictedDates.Length; ++i)
                predictedDates[i] = controlSet.ElementAt(i).Date;

            double[] closeValues = new double[trainingCount];

            for (int i = 0; i < closeValues.Length; ++i)
                closeValues[i] = (double)trainingSet.ElementAt(i).Close;

            Vector<double> closeValuesVector = Vector.Create(closeValues);

            ArimaModel arima = ComputeBestValues(closeValuesVector, 1, 2, 1, 3, 3, 3);

            arima.EstimateMean = true;

            arima.Fit();

            Vector<double> predictedValuesVector = arima.Forecast(controlCount);

            double[] predictedValues = predictedValuesVector.ToArray();

            return ConvertToDatasetRange(predictedDates, predictedValues);
        }

        private static ArimaModel ComputeBestValues(Vector<double> timeSeries, int startP, int startD, int startQ, int maxP, int maxD, int maxQ)
        {
            double bestLikehood = double.MinValue, currLikehood;

            int currP = startP, bestP = currP,
                currD = startD, bestD = currD,
                currQ = startQ, bestQ = currQ;

            while (currP <= maxP)
            {
                currLikehood = GetModelLikelihood(timeSeries, currP, currD, currQ);

                if (bestLikehood < currLikehood)
                {
                    bestLikehood = currLikehood;

                    bestP = currP; bestD = currD; bestQ = currQ;
                }

                if (currQ < maxQ)
                    ++currQ;
                else if (currD < maxD)
                {
                    ++currD;

                    currQ = startQ;
                }
                else
                {
                    ++currP;

                    currD = startD;

                    currQ = startQ;
                }
            }

            return new ArimaModel(timeSeries, bestP, bestD, bestQ);
        }

        private static double GetModelLikelihood(Vector<double> timeSeries, int p, int d, int q)
        {
            ArimaModel model = new ArimaModel(timeSeries, p, d, q)
            {
                EstimateMean = true
            };

            model.Fit();

            return model.LogLikelihood;
        }

        private List<BasicDataset> ConvertToDatasetRange(DateTime[] dates, double[] closes)
        {
            List<BasicDataset> datasets = new List<BasicDataset>(dates.Length);

            for (int i = 0; i < datasets.Capacity; ++i)
            {
                BasicDataset dataset = ConvertToDataset(dates[i], closes[i]);

                datasets.Add(dataset);
            }

            return datasets;
        }

        private BasicDataset ConvertToDataset(DateTime date, double close)
        {
            return new BasicDataset(date, (decimal)close);
        }
    }
}