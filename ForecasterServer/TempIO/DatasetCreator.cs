using Csv;
using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private readonly Dictionary<string, string> extendedHeaders = new Dictionary<string, string>() {
            {"Date", "Date"}, {"High", "High"}, {"Open", "Open"}, {"Close", "Close"}, { "Low","Low" },
            {"Last", "Last" }, {"TotalTradeQuantity", "Total Trade Quantity" }, {"Turnover", "Turnover (Lacs)" }
        };

        private readonly Dictionary<string, string> vitalHeaders = new Dictionary<string, string>() {
            {"Date", "Date"}, {"Close", "Close"}
        };

        public IEnumerable<StockDataset> CreateExtendedFromCsv(IEnumerable<string[]> csvContent)
        {
            IEnumerable<StockDataset> datasets;

            if (csvContent.Count() < 1)
                throw new Exception("File is empty");

            string[] headers = csvContent.ElementAt(0),
                headersToSearch = extendedHeaders.Values.ToArray();

            Dictionary<string, int> headersPosition = CsvReader.GetHeadersPosition(headers, headersToSearch);

            datasets = CreateStockDatasetRange(csvContent, headersPosition);

            return datasets;
        }

        public IEnumerable<BasicDataset> CreateFromCsv(IEnumerable<string[]> csvContent)
        {
            IEnumerable<BasicDataset> datasets;

            if (csvContent.Count() < 1)
                throw new Exception("File is empty");

            string[] headers = csvContent.ElementAt(0),
                headersToSearch = vitalHeaders.Values.ToArray();

            Dictionary<string, int> headersPosition = CsvReader.GetHeadersPosition(headers, headersToSearch);

            datasets = CreateBasicDatasetRange(csvContent, headersPosition);

            return datasets;
        }

        private IEnumerable<StockDataset> CreateStockDatasetRange(IEnumerable<string[]> csvContent, Dictionary<string, int> headersPosition)
        {
            try
            {
                List<StockDataset> datasets = new List<StockDataset>(csvContent.Count());

                foreach (string[] csvLine in csvContent)
                {
                    StockDataset dataset = CreateStockDataset(csvLine, headersPosition);

                    if (dataset != null)
                        datasets.Add(dataset);
                }

                return datasets;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<BasicDataset> CreateBasicDatasetRange(IEnumerable<string[]> csvContent, Dictionary<string, int> headersPosition)
        {
            try
            {
                List<BasicDataset> datasets = new List<BasicDataset>(csvContent.Count());

                foreach (string[] csvLine in csvContent)
                {
                    BasicDataset dataset = CreateBasicDataset(csvLine, headersPosition);

                    if (dataset != null)
                        datasets.Add(dataset);
                }

                return datasets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private StockDataset CreateStockDataset(string[] csvLine, Dictionary<string, int> headersPosition)
        {
            try
            {
                StockDataset dataset = new StockDataset
                {
                    Close = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["Close"]),
                    Last = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["Last"]),
                    Low = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["Low"]),
                    Open = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["Open"]),
                    High = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["High"]),
                    TotalTradeQuantity = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["TotalTradeQuantity"]),
                    Turnover = GetCsvDecimal(csvLine, headersPosition, extendedHeaders["Turnover"]),
                    Date = GetCsvDate(csvLine, headersPosition, extendedHeaders["Date"])
                };

                return dataset;
            }
            catch(IndexOutOfRangeException ex)
            {
                Console.WriteLine("CreateDataset Exception: " + ex.Message);

                return null;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("CreateDataset Exception: " + ex.Message);

                return null;
            }
        }

        private BasicDataset CreateBasicDataset(string[] csvLine, Dictionary<string, int> headersPosition)
        {
            try
            {
                BasicDataset dataset = new BasicDataset
                {
                    Close = GetCsvDecimal(csvLine, headersPosition, vitalHeaders["Close"]),
                    Date = GetCsvDate(csvLine, headersPosition, vitalHeaders["Date"])
                };

                return dataset;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("CreateDataset Exception: " + ex.Message);

                return null;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("CreateDataset Exception: " + ex.Message);

                return null;
            }
        }

        private decimal GetCsvDecimal(string[] csvLine, Dictionary<string, int> headersPosition, string header)
        {
            int headerPosition = headersPosition[header];

            decimal value;
            //try
            //{
                value = decimal.Parse(csvLine[headerPosition], CultureInfo.InvariantCulture);
            //}
            //catch(Exception ex)
            //{
            //    try
            //    {
            //        value = decimal.Parse(csvLine[headerPosition], CultureInfo.InvariantCulture);
            //    }
            //    catch
            //    {
            //        throw ex;
            //    }
            //}

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
