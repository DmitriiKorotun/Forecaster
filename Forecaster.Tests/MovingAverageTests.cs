using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Csv;
using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using Forecaster.Server.Prediction;
using Forecaster.TempIO;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Tests
{
    [TestClass]
    public class MovingAverageTests
    {
        [TestMethod]
        public void Predict_CorrectSet_ExpectedPrediction()
        {
            // arrange
            IPredictionAlgorithm movingAverage = new MovingAverage();

            IEnumerable<StockDataset> dataset = GetDataset("fortests/NSE-TATAGLOBAL11.csv").Reverse();

            // act
            IEnumerable<BasicDataset> prediction = movingAverage.Predict(dataset);

            var expectedPrediction = prediction;

            // assert
            Assert.IsTrue(prediction.Count() > 0);
        }

        private IEnumerable<StockDataset> GetDataset(string pathToCSV)
        {
            List<string[]> csvData = CsvReader.Read(pathToCSV);

            List<StockDataset> dataset = new List<StockDataset>();

            FillList(dataset, csvData, csvData.Count - 1, 1);

            return dataset;
        }

        private static void FillList(List<StockDataset> stockList, List<string[]> inputData, int count, int inputOffset)
        {
            int lastIndex = inputOffset + count;

            for (int i = inputOffset; i < lastIndex; ++i)
                stockList.Add(new StockDataset(inputData[i]));
        }
    }
}
