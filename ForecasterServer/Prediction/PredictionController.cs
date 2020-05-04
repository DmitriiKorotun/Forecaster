using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using Forecaster.Net;
using Forecaster.TempIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.Prediction
{
    public class PredictionController
    {
        public List<BasicDataset> Predict(List<StockDataset> dataset, IPredictionAlgorithm predictionAlgorithm)
        {
            List<BasicDataset> predictedDataset = predictionAlgorithm.Predict(dataset);

            return predictedDataset;
        }

        public List<BasicDataset> Predict(string pathToCSV, IPredictionAlgorithm predictionAlgorithm)
        {
            List<StockDataset> dataset = ReadDataset(pathToCSV);

            return Predict(dataset, predictionAlgorithm);
        }

        private List<StockDataset> ReadDataset(string pathToCSV)
        {
            List<string[]> csvData = Reader.ReadCSV(pathToCSV);

            List<StockDataset> dataset = new List<StockDataset>();

            FillList(dataset, csvData, csvData.Count - 1, 1);

            return dataset;
        }

        private static void FillList(List<BasicDataset> stockList, List<string[]> inputData, int count, int inputOffset)
        {
            int lastIndex = inputOffset + count;

            for (int i = inputOffset; i < lastIndex; ++i)
                stockList.Add(new StockDataset(inputData[i]));
        }

        private static void FillList(List<StockDataset> stockList, List<string[]> inputData, int count, int inputOffset)
        {
            int lastIndex = inputOffset + count;

            for (int i = inputOffset; i < lastIndex; ++i)
                stockList.Add(new StockDataset(inputData[i]));
        }
    }
}
