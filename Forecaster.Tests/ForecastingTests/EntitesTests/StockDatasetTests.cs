using System;
using Forecaster.Forecasting.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Tests.ForecastingTests.EntitesTests
{
    [TestClass]
    public class StockDatasetTests
    {
        [TestMethod]
        public void PropertiesSetters_CorrectFullParamsSet_StockDatasetExpected()
        {
            // arrange
            DateTime date = DateTime.Now;

            decimal close = 100.136246M,
                open = 24.214M, high = 153.325325M,
                low = 8.59275M, last = 68.0553M,
                ttq = 58206, turnover = 492785;

            // act
            StockDataset dataset = new StockDataset
            {
                Date = date,
                Close = close,
                Open = open,
                High = high,
                Low = low,
                Last = last,
                TotalTradeQuantity = ttq,
                Turnover = turnover
            };

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Close, close);
            Assert.AreEqual(dataset.Open, open);
            Assert.AreEqual(dataset.High, high);
            Assert.AreEqual(dataset.Low, low);
            Assert.AreEqual(dataset.Last, last);
            Assert.AreEqual(dataset.TotalTradeQuantity, ttq);
            Assert.AreEqual(dataset.Turnover, turnover);
        }

        [TestMethod]
        public void Constructor_CorrectStockLine_StockDatasetExpected()
        {
            // arrange
            string stockLine = "2017-10-10,100.136246,24.214,153.325325,8.59275,68.0553,58206,492785";

            DateTime date = DateTime.Parse("2017-10-10");

            decimal open = 100.136246M,
                    high = 24.214M, low = 153.325325M,
                    last = 8.59275M, close = 68.0553M,
                    ttq = 58206, turnover = 492785;

            // act
            StockDataset dataset = new StockDataset(stockLine);

            // assert
            Assert.AreEqual(dataset.Date, date);          
            Assert.AreEqual(dataset.Open, open);
            Assert.AreEqual(dataset.High, high);
            Assert.AreEqual(dataset.Low, low);            
            Assert.AreEqual(dataset.Last, last);
            Assert.AreEqual(dataset.Close, close);
            Assert.AreEqual(dataset.TotalTradeQuantity, ttq);
            Assert.AreEqual(dataset.Turnover, turnover);
        }

        [TestMethod]
        public void Constructor_CorrectStockLineWithWhitespaces_StockDatasetExpected()
        {
            // arrange
            string stockLine = " 2017-10-10 , 100.136246 , 24.214 , 153.325325 , 8.59275 , 68.0553 , 58206 , 492785";

            DateTime date = DateTime.Parse("2017-10-10");

            decimal open = 100.136246M,
                    high = 24.214M, low = 153.325325M,
                    last = 8.59275M, close = 68.0553M,
                    ttq = 58206, turnover = 492785;

            // act
            StockDataset dataset = new StockDataset(stockLine);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Open, open);
            Assert.AreEqual(dataset.High, high);
            Assert.AreEqual(dataset.Low, low);
            Assert.AreEqual(dataset.Last, last);
            Assert.AreEqual(dataset.Close, close);
            Assert.AreEqual(dataset.TotalTradeQuantity, ttq);
            Assert.AreEqual(dataset.Turnover, turnover);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_IncorrectStockLine_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "2017-10-10,ttt,24.214,153.325325,8.59275,68.0553,58206,492785";

            // act
            new StockDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_IncorrectStockLineWithIncorrectDataOnDatePlace_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "ttt,100.136246,24.214,153.325325,8.59275,68.0553,58206,492785";

            // act
            new StockDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with comas only was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineWithComasOnly_FormatExceptionExpected()
        {
            // arrange
            string stockLine = ",,,,,,,";

            // act
            new StockDataset(stockLine);
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException), "An incorrect stock line was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_IncompleteIncorrectStockLine_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "ttt";

            // act
            new StockDataset(stockLine);
        }

        [TestMethod]
        public void Constructor_CorrectIncompleteStockLine_StockDatasetExpectedWithDateAndOpenInitialized()
        {
            // arrange
            string stockLine = "2017-10-10,209.11";

            DateTime date = DateTime.Parse("2017-10-10");

            decimal close = 209.11M;

            // act
            StockDataset dataset = new StockDataset(stockLine);

            // assert
            Assert.AreEqual(dataset.Date, date);
            Assert.AreEqual(dataset.Open, close);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with incorrect order was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_IncompleteStockLineWithIncorrectOrder_FormatExceptionExpected()
        {
            // arrange
            string stockLine = "209.11,2017-10-10";

            // act
            new StockDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null stock line was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_NullStockLine_ArgumentNullExceptionExpected()
        {
            // arrange
            string stockLine = null;

            // act
            new StockDataset(stockLine);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "An empty stock line was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_EmptyStockLine_ArgumentNullExceptionExpected()
        {
            // arrange
            string stockLine = string.Empty;

            // act
            new StockDataset(stockLine);
        }

        [TestMethod]
        public void Constructor_CorrectCompleteStockLineValuesArray_StockDatasetExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", "209.11" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with incorrect order was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithIncorrectOrder_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "209.11", "2017-10-10" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with incorrect element was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithIncorrectElementOnDatePlace_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "ttt", "209.11" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with incorrect element was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithIncorrectElementOnOpenPlace_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", "ttt" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with empty elements was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithEmptyElements_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "", "" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with empty element was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithEmptyElementOnOpenPlace_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", "" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with empty element was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithEmptyElementOnDatePlace_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "", "209.11" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with null elements was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithNullElements_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { null, null };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with null element was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithNullElementOnOpenPlace_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10", null };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "A stock line with null element was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_StockLineValuesArrayWithNullElementOnDatePlace_FormatExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { null, "209.11" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        public void Constructor_CorrectIncompleteStockLineValuesArray_StockDatasetExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "2017-10-10" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        public void Constructor_IncompleteStockLineValuesArrayWithIncorrectOrder_StockDatasetWithDateInitializedExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { "209.11" };

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "A null stock line values array was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_NullStockLineValuesArray_ArgumentNullExceptionExpected()
        {
            // arrange
            string[] stockLineValues = null;

            // act
            new StockDataset(stockLineValues);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "An empty stock line values array was inappropriately allowed in StockDataset constructor.")]
        public void Constructor_EmptyStockLineValuesArray_ArgumentExceptionExpected()
        {
            // arrange
            string[] stockLineValues = new string[] { };

            // act
            new StockDataset(stockLineValues);
        }
    }
}
