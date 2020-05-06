using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Forecaster.Forecasting.Prediction;
using Forecaster.Net;
using Forecaster.Net.Requests;
using Forecaster.Server.Local;
using Forecaster.Server.Prediction;

namespace Forecaster.Server
{
    class Program
    {
        static int Main(string[] args)
        {

            //var data = PredictionController.Predict("fortests/NSE-TATAGLOBAL11.csv", new MovingAverage());

            var test = new FileTransferRequest(File.ReadAllBytes("fortests/NSE-TATAGLOBAL11.csv"));

            AsynchronousSocketListener.StartListening();

            return 0;
        }
    }
}