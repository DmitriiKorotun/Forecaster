using Forecaster.Forecasting.Entities;
using Forecaster.Server.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.TempIO
{
    public class DatasetCreator
    {
        //private enum DatasetHeader
        //{
        //    Date,
        //    High,
        //    Open,
        //    Close,
        //    Low,
        //    Last,
        //    TotalTradeQuantity,
        //    Turnover
        //}

        private readonly Dictionary<string, string> datasetHeaders = new Dictionary<string, string>() {
            {"Date", "Date"}, {"High", "High"}, {"Open", "Open"}, {"Close", "Close"}, { "Low","Low" },
            {"Last", "Last" }, {"TotalTradeQuantity", "Total Trade Quantity" }, {"Turnover", "Turnover (Lacs)" }
        };

        public IEnumerable<StockDataset> CreateFromCsv(IEnumerable<string[]> csvContent)
        {
            IEnumerable<StockDataset> datasets;

            if (csvContent.Count() < 1)
                throw new Exception("File is empty");

            string[] headers = csvContent.ElementAt(0),
                headersToSearch = datasetHeaders.Values.ToArray();

            Dictionary<string, int> headersPosition = CsvReader.GetHeadersPosition(headers, headersToSearch);

            datasets = CreateDatasetRange(csvContent, headersPosition);

            return datasets;
        }

        private IEnumerable<StockDataset> CreateDatasetRange(IEnumerable<string[]> csvContent, Dictionary<string, int> headersPosition)
        {
            List<StockDataset> datasets = new List<StockDataset>(csvContent.Count());

            foreach (string[] csvLine in csvContent)
            {
                StockDataset dataset = CreateDataset(csvLine, headersPosition);

                if (dataset != null)
                    datasets.Add(dataset);
            }

            return datasets;
        }

        private StockDataset CreateDataset(string[] csvLine, Dictionary<string, int> headersPosition)
        {
            try
            {
                StockDataset dataset = new StockDataset();

                dataset.Close = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["Close"]);
                dataset.Last = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["Last"]);
                dataset.Low = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["Low"]);
                dataset.Open = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["Open"]);
                dataset.High = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["High"]);
                dataset.TotalTradeQuantity = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["TotalTradeQuantity"]);
                dataset.Turnover = GetCsvDecimal(csvLine, headersPosition, datasetHeaders["Turnover"]);
                dataset.Date = GetCsvDate(csvLine, headersPosition, datasetHeaders["Date"]);

                return dataset;
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        private decimal GetCsvDecimal(string[] csvLine, Dictionary<string, int> headersPosition, string header)
        {
            int headerPosition = headersPosition[header];

            decimal value = decimal.Parse(csvLine[headerPosition]);

            return value;
        }

        private DateTime GetCsvDate(string[] csvLine, Dictionary<string, int> headersPosition, string header)
        {
            int headerPosition = headersPosition[header];

            DateTime value = DateTime.Parse(csvLine[headerPosition]);

            return value;
        }
    }
}
