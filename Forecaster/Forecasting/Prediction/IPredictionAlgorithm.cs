using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Prediction
{
    public interface IPredictionAlgorithm
    {
        List<BasicDataset> Predict(IEnumerable<BasicDataset> dataset);
    }
}
