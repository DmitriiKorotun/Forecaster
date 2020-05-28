using Accord.Math;
using Accord.Statistics.Models.Regression.Linear;
using Forecaster.DataConverters;
using Forecaster.DataHandlers.DatePart;
using Forecaster.DataHandlers.DateTable;
using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Prediction
{
    public class KNearestNeighbors : IPredictionAlgorithm
    {
        public IEnumerable<BasicDataset> Predict(IEnumerable<BasicDataset> datasets)
        {
            SplitSet(datasets, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet);

            int totalCount = datasets.Count(), trainingCount = trainingSet.Count(), controlCount = controlSet.Count();
            //read data
            DataTable mscTable = DataTableConverter.ConvertToDateTable(datasets);

            DateTableHandler dateTableHandler = new DateTableHandler();

            mscTable = dateTableHandler.SortTable(mscTable, "Date ASC");

            IEnumerable<DateTime> dates = mscTable.Columns["Date"].ToArray<DateTime>();

            IEnumerable<DatePart> dateparts = DatePart.CreateFromDates(dates);

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
                new string[] { "Close", "Date", "Open", "High", "Low", "Last", "Total Trade Quantity", "Turnover (Lacs)" }
                );

            var columnsToAdd = CreateNewColumnList();

            dateTableHandler.AddColumnRange(mscTable, columnsToAdd);

            dateTableHandler.FillColumnRange(mscTable, columnsToAdd, dateparts);

            //receiving input data from a table
            double[][] inputs = mscTable.ToJagged(System.Globalization.CultureInfo.InvariantCulture);

            //separation of the test and train sample
            double[][] inputsTrain = inputs.Get(0, trainingCount),
                inputsPredict = inputs.Get(trainingCount, totalCount);

            int[] trainClasses = AssignClasses(outResPositiveTrain, out Dictionary<int, int> valueToClassDictionary);

            int[] predictedClasses = Predict(inputsTrain, trainClasses, inputsPredict);

            double[] predictedValues = GetValuesFromClasses(predictedClasses, valueToClassDictionary);

            return ConvertToDatasetRange(predictedDates, predictedValues);
        }

        private static int[] AssignClasses(int[] values, out Dictionary<int, int> valueToClassDictionary)
        {
            int[] classes = new int[values.Length];

            valueToClassDictionary = new Dictionary<int, int>(classes.Length);

            int currentClass = 0;

            for (int i = 0; i < classes.Length; ++i)
            {
                if (!valueToClassDictionary.ContainsKey(values[i]))
                    valueToClassDictionary.Add(values[i], currentClass++);

                classes[i] = valueToClassDictionary[values[i]];
            }

            return classes;
        }

        private static int[] AssignClasses(double[] values, out Dictionary<int, int> valueToClassDictionary)
        {
            int[] castedValues = new int[values.Length];

            for (int i = 0; i < castedValues.Length; ++i)
                castedValues[i] = (int)values[i];

            int[] classes = AssignClasses(castedValues, out valueToClassDictionary);

            return classes;
        }

        private int[] Predict(double[][] inputsTrain, int[] outputsClasses, double[][] inputsPredict)
        {
            var knn = new Accord.MachineLearning.KNearestNeighbors(k: 4);

            Accord.MachineLearning.KNearestNeighbors kNearestNeighbors = knn.Learn(inputsTrain, outputsClasses);

            return kNearestNeighbors.Transform(inputsPredict);
        }

        private static double[] GetValuesFromClasses(int[] classes, Dictionary<int, int> valueToClassDictionary)
        {
            double[] values = new double[classes.Length];

            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = valueToClassDictionary.FirstOrDefault(x => x.Value == classes[i]).Key;
            }

            return values;
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

        private void SplitSet(IEnumerable<BasicDataset> datasets, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet)
        {
            int datasetCount = datasets.Count(),
                trainingSize = (int)Math.Ceiling(datasetCount * 0.8),
                controlSize = datasetCount - trainingSize;

            trainingSet = datasets.Take(trainingSize);

            controlSet = datasets.Skip(trainingSize).Take(controlSize);
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
    }
}
