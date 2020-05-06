using Forecaster.Forecasting.Prediction;
using Forecaster.Net.Requests;
using Forecaster.Server.Prediction;
using Forecaster.Server.TempIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server
{
    class Controller
    {
        Dictionary<ushort, IPredictionAlgorithm> Algorithms { get; set; } = new Dictionary<ushort, IPredictionAlgorithm>() { {1, new MovingAverage()} };

        public void GetPrediction(FileTransferRequest request)
        {
            IPredictionAlgorithm algorithm = ChosePredictionAlgorithm(request.SelectedAlgortihms);

            PredictionController.Predict(request.FileBytes, algorithm);
        }

        private IPredictionAlgorithm ChosePredictionAlgorithm(ushort algorithmValue)
        {
            IPredictionAlgorithm algorithm;

            //switch (algorithmValue)
            //{
            //    case 1:
            //        algorithm = new MovingAverage();
            //        break;
            //    default:
            //        throw new Exception("Can't find algorithm");
            //}

            if (Algorithms.ContainsKey(algorithmValue))
                algorithm = Algorithms[algorithmValue];
            else
                throw new Exception("Can't find algorithm");

            return algorithm;
        }
    }
}
