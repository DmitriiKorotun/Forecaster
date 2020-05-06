using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using Forecaster.Net;
using Forecaster.Server.TempIO;
using Forecaster.TempIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.Prediction
{
    public static class PredictionController
    {
        public static List<BasicDataset> Predict(List<StockDataset> dataset, IPredictionAlgorithm predictionAlgorithm)
        {
            List<StockDataset> orderedSet = OrderSet(dataset);

            return predictionAlgorithm.Predict(orderedSet);
        }

        public static List<BasicDataset> Predict(string pathToCSV, IPredictionAlgorithm predictionAlgorithm)
        {
            List<string[]> csvData = ReadCSV(pathToCSV);

            return Predict(csvData, predictionAlgorithm);
        }


        public static List<BasicDataset> Predict(byte[] fileBytes, IPredictionAlgorithm predictionAlgorithm)
        {
            List<string[]> csvData = ConvertCSV(fileBytes);

            return Predict(csvData, predictionAlgorithm);
        }

        private static List<BasicDataset> Predict(List<string[]> csvData, IPredictionAlgorithm predictionAlgorithm)
        {
            List<StockDataset> stockList = CreateStockList(csvData, csvData.Count - 2, 1);

            return Predict(stockList, predictionAlgorithm);
        }

        private static List<StockDataset> OrderSet(List<StockDataset> dataset, bool isAscending = true)
        {
            List<StockDataset> orderedSet = dataset.OrderBy(item => item.Date).ToList();

            if (!isAscending)
                orderedSet.Reverse();

            return orderedSet;
        }

        private static List<string[]> ReadCSV(string pathToCSV)
        {
            List<string[]> csvData = Reader.ReadCSV(pathToCSV);

            return csvData;
        }

        private static List<string[]> ConvertCSV(byte[] csvBytes)
        {
            List<string[]> csvData = CsvConverter.Convert(csvBytes).ToList();

            return csvData;
        }

        private static List<StockDataset> CreateStockList(List<string[]> inputData, int count, int inputOffset)
        {
            List<StockDataset> stockList = new List<StockDataset>();

            FillList(stockList, inputData, count, inputOffset);

            return stockList;
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
