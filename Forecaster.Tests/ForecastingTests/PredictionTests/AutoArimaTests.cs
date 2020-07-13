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
    public class AutoArimaTests
    {
        [TestMethod]
        public void Predict_CorrectSet_ExpectedCorrectPrediction()
        {
            // arrange
            IPredictionAlgorithm autoArima = new AutoArima();

            IEnumerable<BasicDataset> dataset = DatasetCreator.GetDataset("fortests/NSE-TATAGLOBAL11.csv"),
                expected = DatasetCreator.GetDataset("fortests/TataglobalArima.csv");

            // act
            IEnumerable<BasicDataset> prediction = autoArima.Predict(dataset),
                roundedPrediction = DataStandardizer.RoundClose(prediction, 2);

            // assert
            CompareLogic compareLogic = new CompareLogic();

            var comparsionResult = compareLogic.Compare(expected, roundedPrediction);

            Assert.IsTrue(comparsionResult.AreEqual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null dataset was inappropriately allowed in autoArima prediction.")]
        public void Predict_NullSet_ExpectedArgumentNullException()
        {
            // arrange
            IPredictionAlgorithm autoArima = new AutoArima();

            IEnumerable<BasicDataset> dataset = null;

            // act
            autoArima.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "An empty dataset was inappropriately allowed in autoArima prediction.")]
        public void Predict_EmptySet_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm autoArima = new AutoArima();

            IEnumerable<BasicDataset> dataset = new List<BasicDataset>();

            // act
            autoArima.Predict(dataset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "A dataset with less then 20 entities was inappropriately allowed in autoArima prediction.")]
        public void Predict_SetWith19Entities_ExpectedArgumentException()
        {
            // arrange
            IPredictionAlgorithm autoArima = new AutoArima();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(19);

            // act
            autoArima.Predict(dataset);
        }

        [TestMethod]
        public void Predict_SetWith20Entities_NoExceptionExpected()
        {
            // arrange
            IPredictionAlgorithm autoArima = new AutoArima();

            IEnumerable<BasicDataset> dataset = DatasetCreator.CreateSet(20);

            // act
            autoArima.Predict(dataset);
        }
    }
}
