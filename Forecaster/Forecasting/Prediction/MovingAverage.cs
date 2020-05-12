using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Prediction
{
    public class MovingAverage : IPredictionAlgorithm
    {
        //public List<BasicDataset> Predict(IEnumerable<BasicDataset> dataset)
        //{
        //    var predcitedStockList = new List<BasicDataset>(daysToPredict);

        //    List<BasicDataset> average = dataset.ToList();

        //    BasicDataset nextPredicted;

        //    for (int i = 0; i < daysToPredict; ++i)
        //    {
        //        nextPredicted = GetSinglePrediction(average);

        //        predcitedStockList.Add(nextPredicted);

        //        average = CreateAverage(average, nextPredicted);
        //    }

        //    return predcitedStockList;
        //}

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

        public List<BasicDataset> Predict(IEnumerable<StockDataset> average)
        {
            SplitSet(average, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet);

            int predictionCount = controlSet.Count();

            List<BasicDataset> predictedSet = new List<BasicDataset>(predictionCount);

            BasicDataset singlePrediction;

            decimal summ = 0;

            for (int i = 0; i < predictionCount; ++i)
            {
                summ = trainingSet.Skip(trainingSet.Count() - predictionCount + i).Sum(x => x.Close) + predictedSet.Sum(x => x.Close);

                //DateTime predictedDate = predictedSet.Count > 0 ? 
                //    predictedSet.ElementAt(predictedSet.Count() - 1).Date.AddDays(1) :
                //    average.ElementAt(average.Count() - 1).Date.AddDays(1);

                DateTime predictedDate = controlSet.ElementAt(i).Date;

                switch (predictedDate.Date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        predictedDate = predictedDate.AddDays(2);
                        break;
                    case DayOfWeek.Sunday:
                        predictedDate = predictedDate.AddDays(1);
                        break;
                }

                singlePrediction = new BasicDataset(predictedDate, summ / predictionCount);

                predictedSet.Add(singlePrediction);
            }

            return predictedSet;
        }

        private void SplitSet(IEnumerable<BasicDataset> dataset, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet)
        {
            int datasetCount = dataset.Count(),
                trainingSize = (int)Math.Ceiling(datasetCount * 0.8),
                controlSize = datasetCount - trainingSize;

            trainingSet = dataset.Take(trainingSize);

            controlSet = dataset.Skip(trainingSize).Take(controlSize);
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
