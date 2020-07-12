using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.DataHandlers.Standartization
{
    public static class DataStandardizer
    {
        public static IEnumerable<BasicDataset> RoundClose(IEnumerable<BasicDataset> datasets, int decimals)
        {
            List<BasicDataset> roundedSets = new List<BasicDataset>(datasets.Count());

            foreach(BasicDataset dataset in datasets)
            {
                BasicDataset roundedSet = new BasicDataset(dataset.Date, decimal.Round(dataset.Close, decimals, MidpointRounding.AwayFromZero));

                roundedSets.Add(roundedSet);
            }

            return roundedSets;
        }
    }
}
