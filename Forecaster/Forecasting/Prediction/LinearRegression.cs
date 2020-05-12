using Forecaster.Forecasting.Entities;
using System;
using System.Linq;
using Accord.Statistics.Models.Regression.Linear;
using Accord.IO;
using Accord.Math;
using System.Data;
using System.Collections.Generic;
using Accord.Math.Optimization.Losses;
using Forecaster.DataConverters;
using Forecaster.DataHandlers.DateTable;

namespace Forecaster.Forecasting.Prediction
{
    public class LinearRegression : IPredictionAlgorithm
    {
        public List<BasicDataset> Predict(IEnumerable<StockDataset> datasets)
        {
            SplitSet(datasets, out IEnumerable<StockDataset> trainingSet, out IEnumerable<StockDataset> controlSet);

            int totalCount = datasets.Count(), trainingCount = trainingSet.Count(), controlCount = controlSet.Count();
            //read data
            DataTable mscTable = DataTableConverter.ConvertToDateTable(datasets);

            DateTableHandler dateTableHandler = new DateTableHandler();

            mscTable = dateTableHandler.SortTable(mscTable, "Date ASC");

            var datepart = GetDatepartColumns(mscTable.Columns["Date"].ToArray<DateTime>());

            //select the target column
            double[] outResPositive = mscTable.Columns["Close"].ToArray();
            DateTime[] allDates = mscTable.Columns["Date"].ToArray<DateTime>(), predictedDates = new DateTime[controlCount];

            for (int i = 0; i < predictedDates.Length; ++i)
            {
                predictedDates[i] = allDates[trainingCount + i];
            }

            // separation of the test and train target sample
            double[] outResPositiveTrain = outResPositive.Get(0, trainingCount);
            double[] outResPositiveTest = outResPositive.Get(trainingCount, totalCount);

            dateTableHandler.RemoveColumnRange(mscTable,
                new string[] { "Close", "Date", "Open", "High", "Low", "Last", "Total Trade Quantity", "Turnover (Lacs)"}
                );

            var columnsToAdd = CreateNewColumnList();

            dateTableHandler.AddColumnRange(mscTable, columnsToAdd);

            dateTableHandler.FillColumnRange(mscTable, columnsToAdd, datepart);

            //receiving input data from a table
            double[][] inputs = mscTable.ToJagged(System.Globalization.CultureInfo.InvariantCulture);

            //separation of the test and train sample
            double[][] inputsTrain = inputs.Get(0, trainingCount);
            double[][] inputsPredict = inputs.Get(trainingCount, totalCount);

            double[] predicted = Predict(inputsTrain, outResPositiveTrain, inputsPredict);

            return ConvertToDatasetRange(predictedDates, predicted);
        }

        private double[] Predict(double[][] inputsTrain, double[] outResPositiveTrain, double[][] inputsPredict)
        {
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };

            MultipleLinearRegression regression = ols.Learn(inputsTrain, outResPositiveTrain);

            return regression.Transform(inputsPredict);
        }

        private BasicDataset ConvertToDataset(DateTime date, decimal close)
        {
            return new BasicDataset(date, close);
        }

        private BasicDataset ConvertToDataset(DateTime date, double close)
        {
            return new BasicDataset(date, (decimal)close);
        }

        private List<BasicDataset> ConvertToDatasetRange(DateTime[] dates, decimal[] closes)
        {
            List<BasicDataset> datasets = new List<BasicDataset>(dates.Length);

            for (int i = 0; i < datasets.Count; ++i)
            {
                BasicDataset dataset = ConvertToDataset(dates[i], closes[i]);

                datasets.Add(dataset);
            }

            return datasets;
        }

        private List<BasicDataset> ConvertToDatasetRange(DateTime[] dates, double[] closes)
        {
            List<BasicDataset> datasets = new List<BasicDataset>(dates.Length);

            for (int i = 0; i < datasets.Capacity; ++i)
            {
                BasicDataset dataset = ConvertToDataset(dates[i], closes[i]);

                datasets.Add(dataset);
            }

            return datasets;
        }

        private void SplitSet(IEnumerable<StockDataset> datasets, out IEnumerable<StockDataset> trainingSet, out IEnumerable<StockDataset> controlSet)
        {
            int datasetCount = datasets.Count(),
                trainingSize = (int)Math.Ceiling(datasetCount * 0.8),
                controlSize = datasetCount - trainingSize;

            trainingSet = datasets.Take(trainingSize);

            controlSet = datasets.Skip(trainingSize).Take(controlSize);
        }

        private static List<double[]> GetDatepartColumns(DateTime[] trainDataDates)
        {
            var datepartList = new List<double[]>(10)
            {
                GetYearColumn(trainDataDates),
                GetMonthColumn(trainDataDates),
                GetDayOfYearColumn(trainDataDates),
                GetDayOfMonthColumn(trainDataDates),
                GetDayOfWeekColumn(trainDataDates),

                GetIsMonthStartColumn(trainDataDates),
                GetIsMonthEndColumn(trainDataDates),
                GetIsYearStartColumn(trainDataDates),
                GetIsYearEndColumn(trainDataDates),
                GetIsMonOrFriColumn(trainDataDates)
            };

            return datepartList;
        }

        private static List<DataColumn> CreateNewColumnList()
        {
            var columnList = new List<DataColumn>(10)
            {
                CreateNewColumns<double>("Year"),
                CreateNewColumns<double>("Month"),
                CreateNewColumns<double>("DayOfYear"),
                CreateNewColumns<double>("DayOfMonth"),
                CreateNewColumns<double>("DayOfWeek"),
                CreateNewColumns<double>("IsMonthStart"),
                CreateNewColumns<double>("IsMonthEnd"),
                CreateNewColumns<double>("IsYearStart"),
                CreateNewColumns<double>("IsYearEnd"),
                CreateNewColumns<double>("IsMonOrFri")
            };

            return columnList;
        }

        private static DataColumn CreateNewColumns<T>(string label)
        {
            DataColumn newCol = new DataColumn(label, typeof(T))
            {
                AllowDBNull = true
            };

            return newCol;
        }

        private static void WritePredicted(double[] predicted, DateTime[] predictedDates)
        {
            var lines = new string[predicted.Length];

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = predictedDates[i].ToString("yyyy-MM-dd") + ',' + predicted[i].ToString(System.Globalization.CultureInfo.InvariantCulture);

            System.IO.File.WriteAllLines("predictedStockList.txt", lines);
        }

        private static double[] GetDayOfYearColumn(DateTime[] trainDataDates)
        {
            double[] daysOfYear = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                daysOfYear[i] = trainDataDates[i].DayOfYear;
            }

            return daysOfYear;
        }

        private static double[] GetDayOfWeekColumn(DateTime[] trainDataDates)
        {
            double[] daysOfYear = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                daysOfYear[i] = (double)trainDataDates[i].DayOfWeek;
            }

            return daysOfYear;
        }

        private static double[] GetDayOfMonthColumn(DateTime[] trainDataDates)
        {
            double[] daysOfMonth = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                daysOfMonth[i] = (double)trainDataDates[i].Day;
            }

            return daysOfMonth;
        }

        private static double[] GetYearColumn(DateTime[] trainDataDates)
        {
            double[] daysOfYear = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                daysOfYear[i] = trainDataDates[i].Year;
            }

            return daysOfYear;
        }

        private static double[] GetMonthColumn(DateTime[] trainDataDates)
        {
            double[] daysOfYear = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                daysOfYear[i] = trainDataDates[i].Month;
            }

            return daysOfYear;
        }

        private static double[] GetIsMonthStartColumn(DateTime[] trainDataDates)
        {
            double[] isMonthStart = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                if (trainDataDates[i].Day < 4)
                    isMonthStart[i] = 1;
                else
                    isMonthStart[i] = 0;
            }

            return isMonthStart;
        }

        private static double[] GetIsMonthEndColumn(DateTime[] trainDataDates)
        {
            double[] isMonthEnd = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                if (trainDataDates[i].Day > 26)
                    isMonthEnd[i] = 1;
                else
                    isMonthEnd[i] = 0;
            }

            return isMonthEnd;
        }

        private static double[] GetIsYearStartColumn(DateTime[] trainDataDates)
        {
            double[] isYearStart = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                if (trainDataDates[i].DayOfYear < 20)
                    isYearStart[i] = 1;
                else
                    isYearStart[i] = 0;
            }

            return isYearStart;
        }

        private static double[] GetIsYearEndColumn(DateTime[] trainDataDates)
        {
            double[] isYearEnd = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                if (trainDataDates[i].DayOfYear > 345)
                    isYearEnd[i] = 1;
                else
                    isYearEnd[i] = 0;
            }

            return isYearEnd;
        }

        private static double[] GetIsMonOrFriColumn(DateTime[] trainDataDates)
        {
            double[] isMonOrFri = new double[trainDataDates.Length];

            for (int i = 0; i < trainDataDates.Length; i++)
            {
                if (trainDataDates[i].DayOfWeek == DayOfWeek.Friday || trainDataDates[i].DayOfWeek == DayOfWeek.Monday)
                    isMonOrFri[i] = 1;
                else
                    isMonOrFri[i] = 0;
            }

            return isMonOrFri;
        }
    }
}