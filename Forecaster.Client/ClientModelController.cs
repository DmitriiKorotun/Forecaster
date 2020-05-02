using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Forecaster.Client
{
    public static class ClientModelController
    {
        public static void WriteOutput(TextBlock tb)
        {
            string toAppend = string.Empty;

            if (Logging.RamLogger.Sb.Length > 0)
            {
                tb.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (!string.IsNullOrEmpty(tb.Text))
                        toAppend += '\n';

                    toAppend += Logging.RamLogger.Flush();

                    tb.Text += toAppend;
                });
            }
        }
    }
}
