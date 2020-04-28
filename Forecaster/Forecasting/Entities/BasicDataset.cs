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
