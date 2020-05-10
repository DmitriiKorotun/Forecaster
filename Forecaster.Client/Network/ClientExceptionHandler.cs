using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Network
{
    class ClientExceptionHandler
    {
        public event EventHandler<ExceptionReportEventArgs> ExceptionReport;

        public List<Exception> RaisedExceptions { get; private set; }

        public ClientExceptionHandler()
        {
            RaisedExceptions = new List<Exception>();
        }

        public ClientExceptionHandler(Exception execption)
        {
            RaisedExceptions = new List<Exception>
            {
                execption
            };
        }

        public ClientExceptionHandler(List<Exception> exceptions)
        {
            RaisedExceptions = exceptions;
        }

        private void OnExceptionReport(object sender, Exception exception)
        {
            ExceptionReport?.Invoke(sender, new ExceptionReportEventArgs(exception));
        }
    }
}
