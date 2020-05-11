using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csv
{
    class CsvWriter
    {
        public static void Write(string path, List<string[]> data)
        {
            string[] separatedData = ConvertToCsvReady(",", data);

            File.WriteAllLines(path, separatedData);
        }

        private static string[] ConvertToCsvReady(string separator, List<string[]> data)
        {
            if (data.Count < 1)
                throw new ArgumentException("Data to write in csv is empty");

            string[] separatedData = new string[data.Count];

            int lineNum = 0;

            foreach (string[] line in data)
            {
                separatedData[lineNum] = string.Join(separator, line);

                ++lineNum;
            }

            return separatedData;
        }
    }
}
