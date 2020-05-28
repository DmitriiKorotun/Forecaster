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
using Forecaster.DataHandlers.DatePart;

namespace Forecaster.Forecasting.Prediction
{
    public class LinearRegression : IPredictionAlgorithm
    {
        public IEnumerable<BasicDataset> Predict(IEnumerable<BasicDataset> datasets)
        {
            SplitSet(datasets, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet);

            int totalCount = datasets.Count(), trainingCount = trainingSet.Count(), controlCount = controlSet.Count();

            DataTable mscTable = GetTableForPrediction(datasets);

            IEnumerable<DateTime> predictedDates = GetPredictionDates(mscTable, trainingCount, controlCount);

            double[] outResPositive = mscTable.Columns["Close"].ToArray(),
                outResPositiveTrain = outResPositive.Get(0, trainingCount);

            AddDatepartColumns(mscTable);

            RemoveUnneededColumns(mscTable);

            double[][] inputs = mscTable.ToJagged(System.Globalization.CultureInfo.InvariantCulture),
                inputsTrain = inputs.Get(0, trainingCount),
                inputsPredict = inputs.Get(trainingCount, totalCount);

            double[] predicted = Predict(inputsTrain, outResPositiveTrain, inputsPredict);

            return ConvertToDatasetRange(predictedDates, predicted);
        }

        private double[] Predict(double[][] inputsTrain, double[] outResPositiveTrain, double[][] inputsPredict)
        {
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true,
                IsRobust = true
            };

            MultipleLinearRegression regression = ols.Learn(inputsTrain, outResPositiveTrain);

            return regression.Transform(inputsPredict);
        }

        private DataTable GetTableForPrediction(IEnumerable<BasicDataset> datasets)
        {
            DateTableHandler dateTableHandler = new DateTableHandler();

            DataTable table = DataTableConverter.ConvertToDateTable(datasets);

            DataTable sortedTable = dateTableHandler.SortTable(table, "Date ASC");

            return sortedTable;
        }

        private IEnumerable<DateTime> GetPredictionDates(DataTable table, int trainingCount, int controlCount)
        {
            DateTime[] allDates = table.Columns["Date"].ToArray<DateTime>();

            List<DateTime> predictedDates = new List<DateTime>(controlCount);

            for (int i = 0; i < predictedDates.Capacity; ++i)
                predictedDates.Add(allDates[trainingCount + i]);

            return predictedDates;
        }

        private void AddDatepartColumns(DataTable table)
        {
            DateTableHandler dateTableHandler = new DateTableHandler();

            IEnumerable<DateTime> dates = table.Columns["Date"].ToArray<DateTime>();

            IEnumerable<DatePart> dateparts = DatePart.CreateFromDates(dates);

            string[] headers = new string[] {
                "Year", "Month", "DayOfYear", "DayOfMonth", "DayOfWeek", "IsMonthStart",
                "IsMonthEnd", "IsYearStart", "IsYearEnd", "IsMonOrFri"
            };

            dateTableHandler.CreateColumnRange<double>(table, headers);

            dateTableHandler.FillColumnRange(table, headers, dateparts);
        }

        private void RemoveUnneededColumns(DataTable table)
        {
            DateTableHandler dateTableHandler = new DateTableHandler();

            dateTableHandler.RemoveColumnRange(table,
                new string[] { "Close", "Date", "Open", "High", "Low", "Last", "Total Trade Quantity", "Turnover (Lacs)" }
                );
        }

        private IEnumerable<BasicDataset> ConvertToDatasetRange(IEnumerable<DateTime> dates, IEnumerable<decimal> closes)
        {
            List<BasicDataset> datasets = new List<BasicDataset>(dates.Count());

            for (int i = 0; i < datasets.Count; ++i)
            {
                BasicDataset dataset = new BasicDataset(dates.ElementAt(i), closes.ElementAt(i));

                datasets.Add(dataset);
            }

            return datasets;
        }

        private IEnumerable<BasicDataset> ConvertToDatasetRange(IEnumerable<DateTime> dates, IEnumerable<double> closes)
        {
            List<BasicDataset> datasets = new List<BasicDataset>(dates.Count());

            for (int i = 0; i < datasets.Capacity; ++i)
            {
                BasicDataset dataset = new BasicDataset(dates.ElementAt(i), closes.ElementAt(i));

                datasets.Add(dataset);
            }

            return datasets;
        }

        private void SplitSet(IEnumerable<BasicDataset> datasets, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet)
        {
            int datasetCount = datasets.Count(),
                trainingSize = (int)Math.Ceiling(datasetCount * 0.8),
                controlSize = datasetCount - trainingSize;

            trainingSet = datasets.Take(trainingSize);

            controlSet = datasets.Skip(trainingSize).Take(controlSize);
        }
    }
}