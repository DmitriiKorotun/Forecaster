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
            var stockStringValues = stockLine.Split(',');

            ApplyStockStringValues(stockStringValues);
        }

        public BasicDataset(string[] stockStringValues)
        {
            ApplyStockStringValues(stockStringValues);
        }

        protected virtual void ApplyStockStringValues(string[] stockStringValues)
        {
            for (int i = 0; i < stockStringValues.Length && i < 8; ++i)
            {
                switch (i)
                {
                    case 0:
                        Date = DateTime.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 1:
                        Close = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                }
            }
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
