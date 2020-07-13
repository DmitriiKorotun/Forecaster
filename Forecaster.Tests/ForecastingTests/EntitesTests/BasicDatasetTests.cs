using System;
using Forecaster.Forecasting.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Tests.ForecastingTests.EntitesTests
{
    [TestClass]
    public class BasicDatasetTests
    {
        [TestMethod]
        public void Constructor_CorrectDataAndDecimal_BasicDatasetExpected()
        {
            // arrange
            DateTime date = DateTime.Now;

            decimal close = 100.136246M;

            // act
            BasicDataset dataset = new BasicDataset(date, close);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, close);
        }

        [TestMethod]
        public void Constructor_CorrectDataAndDouble_BasicDatasetExpected()
        {
            // arrange
            DateTime date = DateTime.Now;

            double close = 100.136246d;

            // act
            BasicDataset dataset = new BasicDataset(date, close);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, (decimal)close);
        }

        [TestMethod]
        public void PropertiesSetters_CorrectDataAndDecimal_BasicDatasetExpected()
        {
            // arrange
            DateTime date = DateTime.Now;

            decimal close = 100.136246M;

            // act
            BasicDataset dataset = new BasicDataset
            {
                Date = date,
                Close = close
            };

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, close);
        }

        [TestMethod]
        public void Constructor_CorrectStockLine_BasicDatasetExpected()
        {
            // arrange
            string stockLine = "2017-10-10,209.11";

            DateTime date = DateTime.Parse("2017-10-10");

            decimal close = 209.11M;

            // act
            BasicDataset dataset = new BasicDataset(stockLine);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, close);
        }

        [TestMethod]
        public void Constructor_CorrectStockLineWithWhitespaces_BasicDatasetExpected()
        {
            // arrange
            string stockLine = " 2017-10-10 , 209.11 ";

            DateTime date = DateTime.Parse("2017-10-10");

            decimal close = 209.11M;

            // act
            BasicDataset dataset = new BasicDataset(stockLine);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, close);
        }

        [TestMethod]
        public void Constructor_CorrectIncompleteStockLine_BasicDatasetExpected()
        {
            // arrange
            string stockLine = "2017-10-10";

            DateTime date = DateTime.Parse("2017-10-10");

            // act
            BasicDataset dataset = new BasicDataset(stockLine);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with incorrect order was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineWithIncorrectOrder_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "209.11,2017-10-10";

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        public void Constructor_IncompleteStockLineWithIncorrectOrder_BasicDatasetWithDateInitializedExpected()
        {
            // arrange
            string stockLine = "209.11";

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_IncorrectStockLine_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "2017-10-10,ttt";

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_IncorrectStockLineWithIncorrectOrder_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "ttt,2017-10-10";

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_IncorrectIncompleteStockLine_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "ttt";

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineWithComasOnly_FormatExceptionExpected()
        {
            // arrange
            string stockLine = ",";

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null stock line was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_NullStockLine_ArgumentNullExceptionExpected()
        {
            // arrange
            string stockLine = null;

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "An empty stock line was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_EmptyStockLine_ArgumentNullExceptionExpected()
        {
            // arrange
            string stockLine = string.Empty;

            // act
            new BasicDataset(stockLine);
        }

        [TestMethod]
        public void Constructor_CorrectCompleteStockLineValuesArray_BasicDatasetExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", "209.11" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with incorrect order was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithIncorrectOrder_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "209.11", "2017-10-10" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with empty strings was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithEmptyStrings_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "", "" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with one empty string was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithOnlyOneEmptyString_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", "" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with null strings was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithNullStrings_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { null, null };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with one null string was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithOnlyOneNullString_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", null };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        public void Constructor_CorrectIncompleteStockLineValuesArray_BasicDatasetExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        public void Constructor_IncompleteStockLineValuesArrayWithIncorrectOrder_BasicDatasetWithDateInitializedExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "209.11" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with empty string was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithEmptyString_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "" };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with null string was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithNullString_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { null };

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null stock line values array was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_NullStockLineValuesArray_ArgumentNullExceptionExpected()
        {
            // arrange
            string[] stockLineValues = null;

            // act
            new BasicDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "An empty stock line values array was inappropriately allowed in BasicDataset constructor.")]
        public void Constructor_EmptyStockLineValuesArray_ArgumentExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { };

            // act
            new BasicDataset(stockLineValues);
        }
    }
}
