using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Entities
{
    public class BasicDataset
    {
        public DateTime Date { get; set; }
        public decimal Close { get; set; }

        public BasicDataset() { }

        public BasicDataset(DateTime date, decimal close)
        {
            Date = date;
            Close = close;
        }

        public BasicDataset(DateTime date, double close)
        {
            Date = date;
            Close = (decimal)close;
        }

        public BasicDataset(string stockLine)
        {
            if (string.IsNullOrEmpty(stockLine))
                throw new ArgumentNullException("stockLine string is null or empty; called in BasicDataset constructor");

            var stockStringValues = stockLine.Split(',');

            ApplyStockStringValues(stockStringValues);
        }

        public BasicDataset(string[] stockStringValues)
        {
            if (stockStringValues is null)
                throw new ArgumentNullException("stockStringValues is null and can't be used to initialize BasicDataset");
            else if (stockStringValues.Length < 1)
                throw new ArgumentException("stockStringValues is empty and can't be used to initialize BasicDataset");

            ApplyStockStringValues(stockStringValues);
        }

        protected virtual void ApplyStockStringValues(string[] stockStringValues)
        {
            for (int i = 0; i < stockStringValues.Length && i < 2; ++i)
            {
                switch (i)
                {
                    case 0:
                        {
                            Date = ParseDate(stockStringValues[i], CultureInfo.InvariantCulture);
                            break;
                        }
                    case 1:
                        {
                            Close = ParseDecimal(stockStringValues[i], CultureInfo.InvariantCulture);
                            break;
                        }
                }
            }
        }

        protected DateTime ParseDate(string dateStr, IFormatProvider provider)
        {
            bool isSucceeded = DateTime.TryParse(dateStr, provider, DateTimeStyles.None, out DateTime date);

            if (!isSucceeded)
                throw new FormatException("Couldn't convert date from stock string value to initialize BasicDataset");

            return date;
        }

        protected decimal ParseDecimal(string decimalStr, IFormatProvider provider)
        {
            bool isSucceeded = decimal.TryParse(decimalStr, NumberStyles.Number, provider, out decimal value);

            if (!isSucceeded)
                throw new FormatException("Couldn't convert date from stock string value to initialize BasicDataset");

            return value;
        }

        public override string ToString()
        {
            string datasetString = "";

            datasetString += GetStringRepresentation(Date, true);
            datasetString += GetStringRepresentation(Close);

            return datasetString;
        }

        protected string GetStringRepresentation(decimal value, bool addSeparator = false)
        {
            return addSeparator ? value.ToString(CultureInfo.InvariantCulture) + ',' : value.ToString(CultureInfo.InvariantCulture);
        }

        protected string GetStringRepresentation(DateTime date, bool addSeparator = false)
        {
            return addSeparator ? date.ToString("yyyy-MM-dd") + ',' : date.ToString("yyyy-MM-dd");
        }
    }
}
