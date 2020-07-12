using Csv;
using Forecaster.DataHandlers;
using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Tests
{
    public static class DatasetCreator
    {
        public static IEnumerable<BasicDataset> GetDataset(string pathToCSV)
        {
            List<string[]> csvData = CsvReader.Read(pathToCSV);

            Server.TempIO.DatasetCreator datasetCreator = new Server.TempIO.DatasetCreator();

            IEnumerable<BasicDataset> dataset = datasetCreator.CreateFromCsv(csvData),
                orderedSet = DatasetHandler.OrderByDate(dataset);

            return orderedSet;
        }

        public static IEnumerable<BasicDataset> CreateSet(int entitiesNum)
        {
            List<BasicDataset> dataset = new List<BasicDataset>(entitiesNum);

            Random rand = new Random();

            for (int i = 0; i < entitiesNum; ++i)
            {
                BasicDataset entity = new BasicDataset(DateTime.Now.AddDays(i - 2000), (double)rand.Next(10, 11 + i));

                dataset.Add(entity);
            }

            return dataset;
        }
    }
}
