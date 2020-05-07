using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Logging
{
    public static class RamLogger
    {
        public static StringBuilder Sb { get; set; } = new StringBuilder();

        public static void Log(string message)
        {
            if (Sb.Length > 0)
                Sb.Append('\n');

            Sb.Append(message);
        }

        public static void Clear()
        {
            Sb.Clear();
        }

        public static string Flush()
        {
            string log = Sb.ToString();

            Clear();

            return log;
        }
    }
}
