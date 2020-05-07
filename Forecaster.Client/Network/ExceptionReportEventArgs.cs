using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Network
{
    public class ExceptionReportEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionReportEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
