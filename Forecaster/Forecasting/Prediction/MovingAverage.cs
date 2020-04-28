using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Prediction
{
    class MovingAverage
    {
        public List<BasicDataset> Predict(IEnumerable<BasicDataset> dataset, int daysToPredict)
        {
            var predcitedStockList = new List<BasicDataset>(daysToPredict);

            List<BasicDataset> average = dataset.ToList();

            BasicDataset nextPredicted;

            for (int i = 0; i < daysToPredict; ++i)
            {
                nextPredicted = GetSinglePrediction(average);

                predcitedStockList.Add(nextPredicted);

                average = CreateAverage(average, nextPredicted);
            }

            return predcitedStockList;
        }

        private BasicDataset GetSinglePrediction(IEnumerable<BasicDataset> average)
        {
            BasicDataset predictedData = new BasicDataset();

            decimal summ = 0;

            foreach (BasicDataset stockData in average)
            {
                summ += stockData.Close;
            }

            predictedData.Close = summ / average.Count();

            predictedData.Date = average.ElementAt(average.Count() - 1).Date.AddDays(1);

            return predictedData;
        }

        public List<BasicDataset> GetAllPrediction(IEnumerable<BasicDataset> average, int predictionCount)
        {
            List<BasicDataset> preds = new List<BasicDataset>();

            BasicDataset predictedData;

            decimal summ = 0;

            for (int i = 0; i < predictionCount; ++i)
            {
                summ = average.Skip(average.Count() - predictionCount + i).Sum(x => x.Close) + preds.Sum(x => x.Close);

                DateTime predictedDate = preds.Count > 0 ? preds.ElementAt(preds.Count() - 1).Date.AddDays(1) : average.ElementAt(average.Count() - 1).Date.AddDays(1);

                switch(predictedDate.Date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        predictedDate = predictedDate.AddDays(2);
                        break;
                    case DayOfWeek.Sunday:
                        predictedDate = predictedDate.AddDays(1);
                        break;
                }

                predictedData = new BasicDataset(predictedDate, summ / predictionCount);

                preds.Add(predictedData);
            }

            return preds;
        }

        private List<BasicDataset> CreateAverage(IEnumerable<BasicDataset> oldAverage, BasicDataset nextPredicted)
        {
            List<BasicDataset> average = oldAverage.ToList();

            if (average != null)
            {
                if (average.Count() > 0)
                    average.RemoveAt(0);

                average.Add(nextPredicted);
            }
            else
                throw new Exception("Old average not initialized");

            return average;
        }
    }
}
