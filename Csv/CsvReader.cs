using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csv
{
    public class CsvReader
    {
        public static List<string[]> Read(string filename)
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

        public static IEnumerable<string[]> ReadFromBytes(byte[] csvBytes)
        {
            List<string[]> converted = new List<string[]>();

            string[] lines = Encoding.UTF8.GetString(csvBytes).Split('\n');

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                converted.Add(values);
            }

            return converted;
        }

        public static Dictionary<string, int> GetHeadersPosition(string[] csvHeaders, string[] headersToSearch, bool isRegisterIgnored = true)
        {
            Dictionary<string, int> headersPosition = new Dictionary<string, int>(headersToSearch.Length);

            int position = 0;

            foreach (string header in csvHeaders)
            {
                foreach (string headerToSearch in headersToSearch)
                {
                    bool isHeadersEqual = isRegisterIgnored ? header.ToLower() == headerToSearch.ToLower() : header == headerToSearch;

                    if (isHeadersEqual)
                    {
                        AddHeader(headersPosition, headerToSearch, position);

                        break;
                    }
                }
                //switch (header)
                //{
                //    case "Date":
                //    case "Open":
                //    case "High":
                //    case "Low":
                //    case "Last":
                //    case "Close":
                //    case "Total Trade Quantity":
                //    case "Turnover (Lacs)":
                //        AddHeader(headersPosition, header, position);
                //        break;
                //}

                ++position;
            }

            if (headersPosition.Count < headersToSearch.Length)
                throw new Exception("Missing header(s)");

            return headersPosition;
        }

        private static void AddHeader(Dictionary<string, int> headersPosition, string header, int position)
        {
            if (!headersPosition.ContainsKey(header))
                headersPosition.Add(header, position);
        }
    }
}
