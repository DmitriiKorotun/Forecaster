using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.CSV
{
    public static class CsvConverter
    {
        public static Dictionary<DateTime, double> ConvertToDictionary(List<string[]> csvContent)
        {
            if (csvContent.Count < 1)
                throw new Exception("Csv content is empty");

            GetHeadersPosition(csvContent[0], out int datePosition, out int closePosition);

            // Removing line with headers
            csvContent.RemoveAt(0);

            return ConvertToDictionary(csvContent, datePosition, closePosition);
        }

        private static void GetHeadersPosition(string[] csvHeaders, out int datePosition, out int closePosition)
        {
            datePosition = -1; closePosition = -1;

            int currentHeaderPos = 0;

            foreach (string csvHeader in csvHeaders)
            {
                switch (csvHeader)
                {
                    case "Date":
                        datePosition = currentHeaderPos;
                        break;
                    case "Close":
                        closePosition = currentHeaderPos;
                        break;
                }

                ++currentHeaderPos;
            }

            if (datePosition < 0 || closePosition < 0)
                throw new Exception("Cannot find important headers in csv file");
        }

        private static void GetHeadersPosition(string csvHeadersString, out int datePosition, out int closePosition)
        {
            string[] csvHeaders = csvHeadersString.Split(',');

            GetHeadersPosition(csvHeaders, out datePosition, out closePosition);
        }

        private static Dictionary<DateTime, double> ConvertToDictionary(List<string[]> csvContent, int datePosition, int closePosition)
        {
            Dictionary<DateTime, double> dateCloseDictionary = new Dictionary<DateTime, double>(csvContent.Count);

            foreach(string[] csvRow in csvContent)
            {
                DateTime date = DateTime.Parse(csvRow[datePosition]);

                double close = double.Parse(csvRow[closePosition], CultureInfo.InvariantCulture);

                dateCloseDictionary.Add(date, close);
            }

            return dateCloseDictionary;
        }
    }
}
