using System;
using System.Collections.Generic;
using Forecaster.DataHandlers.Standartization;
using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Tests.ForecastingTests.PredictionTests
{
    [TestClass]
    public class KNearestNeighboursTests
    {
        [TestMethod]
        public void Predict_CorrectSet_ExpectedCorrectPrediction()
        {
            // arrange
            IPredictionAlgorithm kNearestNeighbours = new KNearestNeighbors();

            IEnumerable<BasicDataset> dataset = DatasetCreator.GetDataset("fortests/NSE-TATAGLOBAL11.csv"),
                expected = DatasetCreator.GetDataset("fortests/TataglobalKNN.csv");

            // act
            IEnumerable<BasicDataset> prediction = kNearestNeighbours.Predict(dataset),
                roundedPrediction = DataStandardizer.RoundClose(prediction, 2);

            // assert
            CompareLogic compareLogic = new CompareLogic();

            var comparsionResult = compareLogic.Compare(expected, roundedPrediction);

            Assert.IsTrue(comparsionResult.AreEqual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null dataset was inappropriately allowed in kNearestNeighbours prediction.")]
        public void Predict_NullSet_ExpectedArgumentNullException()
        {
            // arrange
            IPredictionAlgorithm kNearestNeighbours = new KNearestNeighbors();

            IEnumerable<BasicDataset> dataset = null;

            // act
            kNearestNeighbours.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "An empty dataset was inappropriately allowed in kNearestNeighbours prediction.")]
        public void Predict_EmptySet_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm kNearestNeighbours = new KNearestNeighbors();

            IEnumerable<BasicDataset> dataset = new List<BasicDataset>();

            // act
            kNearestNeighbours.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "A dataset with less then 20 entities was inappropriately allowed in kNearestNeighbours prediction.")]
        public void Predict_SetWith19Entities_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm kNearestNeighbours = new KNearestNeighbors();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(19);

            // act
            kNearestNeighbours.Predict(dataset);
        }

        [TestMethod]
        public void Predict_SetWith20Entities_NoExceptionExpected()
        {
            // arrange
            IPredictionAlgorithm kNearestNeighbours = new KNearestNeighbors();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(20);

            // act
            kNearestNeighbours.Predict(dataset);
        }
    }
}
