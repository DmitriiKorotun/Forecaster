using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.TempIO
{
    static class CsvConverter
    {
        public static IEnumerable<string[]> Convert(byte[] csvBytes)
        {
            List<string[]> converted = new List<string[]>();

            string[] lines = Encoding.UTF8.GetString(csvBytes).Split('\n');

            foreach(string line in lines)
            {
                string[] values = line.Split(',');

                converted.Add(values);
            }

            return converted;
        }
    }
}
