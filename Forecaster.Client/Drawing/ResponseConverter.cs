using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Drawing
{
    public static class ResponseConverter
    {
        public static Dictionary<DateTime, double> ConvertResponsePredictions(Dictionary<string, string> responsePredictions)
        {
            Dictionary<DateTime, double> convertedPredictions = new Dictionary<DateTime, double>(responsePredictions.Count);

            foreach(KeyValuePair<string, string> prediction in responsePredictions)
            {
                KeyValuePair<DateTime, double> convertedPrediction = ConvertResponsePrediction(prediction);

                convertedPredictions.Add(convertedPrediction.Key, convertedPrediction.Value);
            }

            return convertedPredictions;
        }
        
        private static KeyValuePair<DateTime, double> ConvertResponsePrediction(KeyValuePair<string, string> prediction)
        {
            DateTime date = DateTime.Parse(prediction.Key);

            double close = double.Parse(prediction.Value);

            return new KeyValuePair<DateTime, double>(date, close);
        }
    }
}
