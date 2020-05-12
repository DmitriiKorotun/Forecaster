using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;
using Forecaster.Net;
using Forecaster.Net.Requests;
using Forecaster.Net.Responses;
using Forecaster.Server.Network;
using Forecaster.Server.Prediction;
using Forecaster.Server.TempIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Forecaster.Server
{
    public static class Controller
    {
        static Dictionary<ushort, IPredictionAlgorithm> Algorithms { get; set; } =
            new Dictionary<ushort, IPredictionAlgorithm>() {
                {1, new MovingAverage()}, {2, new LinearRegression() } 
            };

        public static byte[] GetResponse(byte[] receivedData)
        {
            FileTransferRequest request = RequestHandler.RestoreRequest<FileTransferRequest>(receivedData);

            List<BasicDataset> predictions = GetPrediction(request);

            PredictionResponse response = CreateResponse(predictions);

            return ResponseManager.CreateByteResponse(response);
        }

        private static PredictionResponse CreateResponse(List<BasicDataset> predictions)
        {
            PredictionResponse response;

            Dictionary<string, string> dateClosePredictions = ConvertToDateCloseDictionary(predictions);

            response = new PredictionResponse((int)ResponseCode.OK, dateClosePredictions);

            return response;
        }

        private static Dictionary<string, string> ConvertToDateCloseDictionary(List<BasicDataset> predictions)
        {
            Dictionary<string, string> datePricePredictions = new Dictionary<string, string>(predictions.Count);

            foreach (BasicDataset prediction in predictions)
            {
                string date = prediction.Date.ToString("yyyy-MM-dd"),
                    close = string.Format("{0:0.00}", prediction.Close);

                datePricePredictions.Add(date, close);
            }

            return datePricePredictions;
        }

        private static List<BasicDataset> GetPrediction(FileTransferRequest request)
        {
            IPredictionAlgorithm algorithm = ChosePredictionAlgorithm(request.SelectedAlgortihms);

            return PredictionController.Predict(request.FileBytes, algorithm);
        }

        private static IPredictionAlgorithm ChosePredictionAlgorithm(ushort algorithmValue)
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
