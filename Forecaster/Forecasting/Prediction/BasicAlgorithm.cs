using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Forecasting.Prediction
{
    public abstract class BasicAlgorithm
    {
        protected virtual void CheckIncomingSet(IEnumerable<BasicDataset> dataset)
        {
            if (dataset is null)
                throw new ArgumentNullException("Incoming dataset is null");
            else if (dataset.Count() < 20)
                throw new ArgumentException("Incoming dataset contains less than 20 values");
        }
    }
}
