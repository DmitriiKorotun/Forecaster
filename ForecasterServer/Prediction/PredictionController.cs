using Accord;
using Csv;
using Forecaster.DataHandlers;
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
        public static IEnumerable<BasicDataset> Predict(List<BasicDataset> dataset, IPredictionAlgorithm predictionAlgorithm)
        {
            List<BasicDataset> orderedSet = DatasetHandler.OrderByDate(dataset).ToList();

            return predictionAlgorithm.Predict(orderedSet);
        }

        public static IEnumerable<BasicDataset> Predict(string pathToCSV, IPredictionAlgorithm predictionAlgorithm)
        {
            List<string[]> csvData = ReadCSV(pathToCSV);

            return Predict(csvData, predictionAlgorithm);
        }

        public static IEnumerable<BasicDataset> Predict(string pathToCSV, IPredictionAlgorithm predictionAlgorithm, out IEnumerable<BasicDataset> datasets)
        {
            List<string[]> csvData = ReadCSV(pathToCSV);

            return Predict(csvData, predictionAlgorithm, out datasets);
        }


        public static IEnumerable<BasicDataset> Predict(byte[] fileBytes, IPredictionAlgorithm predictionAlgorithm)
        {
            List<string[]> csvData = ConvertCSV(fileBytes);

            return Predict(csvData, predictionAlgorithm);
        }

        private static IEnumerable<BasicDataset> Predict(List<string[]> csvData, IPredictionAlgorithm predictionAlgorithm)
        {
            DatasetCreator datasetCreator = new DatasetCreator();

            List<BasicDataset> stockList = datasetCreator.CreateFromCsv(csvData).ToList();

            return Predict(stockList, predictionAlgorithm);
        }

        private static IEnumerable<BasicDataset> Predict(List<string[]> csvData, IPredictionAlgorithm predictionAlgorithm, out IEnumerable<BasicDataset> datasets)
        {
            DatasetCreator datasetCreator = new DatasetCreator();

            datasets = datasetCreator.CreateFromCsv(csvData).ToList();

            return Predict(datasets.ToList(), predictionAlgorithm);
        }

        private static List<string[]> ReadCSV(string pathToCSV)
        {
            List<string[]> csvData = CsvReader.Read(pathToCSV);

            return csvData;
        }

        private static List<string[]> ConvertCSV(byte[] csvBytes)
        {
            List<string[]> csvData = CsvReader.ReadFromBytes(csvBytes).ToList();

            return csvData;
        }

        private static List<BasicDataset> CreateStockList(List<string[]> inputData, int count, int inputOffset)
        {
            List<BasicDataset> stockList = new List<BasicDataset>();

            FillList(stockList, inputData, count, inputOffset);

            return stockList;
        }

        private static void FillList(List<BasicDataset> stockList, List<string[]> inputData, int count, int inputOffset)
        {
            int lastIndex = inputOffset + count;

            for (int i = inputOffset; i < lastIndex; ++i)
                stockList.Add(new BasicDataset(inputData[i]));
        }

        private static void FillList(List<StockDataset> stockList, List<string[]> inputData, int count, int inputOffset)
        {
            int lastIndex = inputOffset + count;

            for (int i = inputOffset; i < lastIndex; ++i)
                stockList.Add(new StockDataset(inputData[i]));
        }
    }
}
