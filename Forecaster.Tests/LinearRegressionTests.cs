using System;
using System.Collections.Generic;
using Forecaster.DataHandlers.Standartization;
using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Tests
{
    [TestClass]
    public class LinearRegressionTests
    {
        [TestMethod]
        public void Predict_CorrectSet_ExpectedCorrectPrediction()
        {
            // arrange
            IPredictionAlgorithm linearRegression = new LinearRegression();

            IEnumerable<BasicDataset> dataset = DatasetCreator.GetDataset("fortests/NSE-TATAGLOBAL11.csv"),
                expected = DatasetCreator.GetDataset("fortests/TataglobalLR.csv");

            // act
            IEnumerable<BasicDataset> prediction = linearRegression.Predict(dataset),
                roundedPrediction = DataStandardizer.RoundClose(prediction, 2);

            // assert
            CompareLogic compareLogic = new CompareLogic();

            var comparsionResult = compareLogic.Compare(expected, roundedPrediction);

            Assert.IsTrue(comparsionResult.AreEqual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null dataset was inappropriately allowed in linearRegression prediction.")]
        public void Predict_NullSet_ExpectedArgumentNullException()
        {
            // arrange
            IPredictionAlgorithm linearRegression = new LinearRegression();

            IEnumerable<BasicDataset> dataset = null;

            // act
            linearRegression.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "An empty dataset was inappropriately allowed in linearRegression prediction.")]
        public void Predict_EmptySet_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm linearRegression = new LinearRegression();

            IEnumerable<BasicDataset> dataset = new List<BasicDataset>();

            // act
            linearRegression.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "A dataset with less then 20 entities was inappropriately allowed in linearRegression prediction.")]
        public void Predict_SetWith19Entities_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm linearRegression = new LinearRegression();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(19);

            // act
            linearRegression.Predict(dataset);
        }

        [TestMethod]
        public void Predict_SetWith20Entities_NoExceptionExpected()
        {
            // arrange
            IPredictionAlgorithm linearRegression = new LinearRegression();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(20);

            // act
            linearRegression.Predict(dataset);
        }
    }
}
