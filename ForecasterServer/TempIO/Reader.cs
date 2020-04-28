using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.TempIO
{
    public static class Reader
    {
        public static List<string[]> ReadCSV(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                List<string[]> csvList = new List<string[]>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    csvList.Add(values);
                }

                return csvList;
            }
        }
    }
}