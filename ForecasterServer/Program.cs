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
using Forecaster.Server.Scoring;

namespace Forecaster.Server
{
    class Program
    {
        static int Main(string[] args)
        {

            //var data = PredictionController.Predict("fortests/NSE-TATAGLOBAL11.csv", new MovingAverage());

            //var test = new FileTransferRequest(File.ReadAllBytes("fortests/NSE-TATAGLOBAL11.csv"));

            AsynchronousSocketListener.StartListening();

            //IEnumerable<IPredictionAlgorithm> algorithms = new List<IPredictionAlgorithm>() {
            //    new MovingAverage(), new LinearRegression(), new KNearestNeighbors(), new AutoArima()
            //};

            //var ls = Scorer.CalculateAverageRMSE("C:/Users/korot/source/repos/Forecaster/Forecaster.Client/bin/Debug/fortests",
            //    algorithms, out double averagePredictedCount, out double averageActualPrice);

            //string results = "MovingAverage: " + ls[0].ToString() + " averagePredictedCount: " + averagePredictedCount + " averageActualPrice: " + averageActualPrice.ToString() + "\n" +
            //    "LinearRegression: " + ls[1].ToString() + " averagePredictedCount: " + averagePredictedCount + " averageActualPrice: " + averageActualPrice.ToString() + "\n" +
            //    "KNearestNeighbours: " + ls[2].ToString() + " averagePredictedCount: " + averagePredictedCount + " averageActualPrice: " + averageActualPrice.ToString() + "\n" +
            //    "AutoARIMA: " + ls[3].ToString() + " averagePredictedCount: " + averagePredictedCount + " averageActualPrice: " + averageActualPrice.ToString() + "\n";

            //System.IO.File.WriteAllText("rmseScoring1.txt", results);

            return 0;
        }
    }
}