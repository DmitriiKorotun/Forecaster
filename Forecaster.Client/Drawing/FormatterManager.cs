using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Drawing
{
    public static class FormatterManager
    {
        public static Func<double, string> CreateFormatter()
        {
            var dayConfig = Mappers.Xy<DateModel>().X(dateModel => dateModel.Date.Ticks / TimeSpan.FromDays(1).Ticks).Y(dateModel => dateModel.Value);

            Func<double, string> formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("y");

            return formatter;
        }
    }
}
