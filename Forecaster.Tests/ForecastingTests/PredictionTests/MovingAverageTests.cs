using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Csv;
using Forecaster.DataHandlers;
using Forecaster.DataHandlers.Standartization;
using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using Forecaster.Server.TempIO;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Tests.ForecastingTests.PredictionTests
{
    [TestClass]
    public class MovingAverageTests
    {
        [TestMethod]
        public void Predict_CorrectSet_ExpectedCorrectPrediction()
        {
            // arrange
            IPredictionAlgorithm movingAverage = new MovingAverage();

            IEnumerable<BasicDataset> dataset = DatasetCreator.GetDataset("fortests/NSE-TATAGLOBAL11.csv"),
                expected = DatasetCreator.GetDataset("fortests/TataglobalMA.csv");

            // act
            IEnumerable<BasicDataset> prediction = movingAverage.Predict(dataset),
                roundedPrediction = DataStandardizer.RoundClose(prediction, 2);

            // assert
            CompareLogic compareLogic = new CompareLogic();

            var comparsionResult = compareLogic.Compare(expected, roundedPrediction);

            Assert.IsTrue(comparsionResult.AreEqual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null dataset was inappropriately allowed in movingAverage prediction.")]
        public void Predict_NullSet_ExpectedArgumentNullException()
        {
            // arrange
            IPredictionAlgorithm movingAverage = new MovingAverage();

            IEnumerable<BasicDataset> dataset = null;

            // act
            movingAverage.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "An empty dataset was inappropriately allowed in movingAverage prediction.")]
        public void Predict_EmptySet_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm movingAverage = new MovingAverage();

            IEnumerable<BasicDataset> dataset = new List<BasicDataset>();

            // act
            movingAverage.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "A dataset with less then 20 entities was inappropriately allowed in movingAverage prediction.")]
        public void Predict_SetWith19Entities_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm movingAverage = new MovingAverage();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(19);

            // act
            movingAverage.Predict(dataset);
        }

        [TestMethod]
        public void Predict_SetWith20Entities_NoExceptionExpected()
        {
            // arrange
            IPredictionAlgorithm movingAverage = new MovingAverage();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(20);

            // act
            movingAverage.Predict(dataset);
        }
    }
}
