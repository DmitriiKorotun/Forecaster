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
            string formatter(double value) => value < 0.0 ? new DateTime((long)0.0).ToString("y") : new DateTime(
                (long)(value * TimeSpan.FromDays(1).Ticks)).ToString("y");

            return formatter;
        }

        private static string Test(double value)
        {
            if (value >= 0)
                return new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("y");
            else
                throw new Exception();
        }
    }
}
