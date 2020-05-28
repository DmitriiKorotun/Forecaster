using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Entities
{
    public class StockDataset : BasicDataset
    { 
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Last { get; set; }
        
        public decimal TotalTradeQuantity { get; set; }
        public decimal Turnover { get; set; }

        public StockDataset() { }

        public StockDataset(string stockLine) : base(stockLine)
        {

        }

        public StockDataset(string[] stockStringValues) : base(stockStringValues)
        {

        }

        protected override void ApplyStockStringValues(string[] stockStringValues)
        {
            for (int i = 0; i < stockStringValues.Length && i < 8; ++i)
            {
                switch (i)
                {
                    case 0:
                        Date = DateTime.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 1:
                        Open = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 2:
                        High = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 3:
                        Low = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 4:
                        Last = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 5:
                        Close = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 6:
                        TotalTradeQuantity = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                    case 7:
                        Turnover = decimal.Parse(stockStringValues[i], CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        public override string ToString()
        {
            string datasetString = "";

            datasetString += GetStringRepresentation(Date, true);
            datasetString += GetStringRepresentation(Open, true);
            datasetString += GetStringRepresentation(High, true);
            datasetString += GetStringRepresentation(Low, true);
            datasetString += GetStringRepresentation(Last, true);
            datasetString += GetStringRepresentation(Close, true);
            datasetString += GetStringRepresentation(TotalTradeQuantity, true);
            datasetString += GetStringRepresentation(Turnover);

            return datasetString;
        }
    }
}