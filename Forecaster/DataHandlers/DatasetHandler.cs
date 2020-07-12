using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.DataHandlers
{
    public static class DatasetHandler
    {
        public static IEnumerable<BasicDataset> OrderByDate(IEnumerable<BasicDataset> dataset, bool isAscending = true)
        {
            List<BasicDataset> orderedSet = dataset.OrderBy(item => item.Date).ToList();

            if (!isAscending)
                orderedSet.Reverse();

            return orderedSet;
        }

        public static void SplitSet(IEnumerable<BasicDataset> dataset, out IEnumerable<BasicDataset> trainingSet, out IEnumerable<BasicDataset> controlSet)
        {
            int datasetCount = dataset.Count(),
                trainingSize = (int)Math.Ceiling(datasetCount * 0.8),
                controlSize = datasetCount - trainingSize;

            trainingSet = dataset.Take(trainingSize);

            controlSet = dataset.Skip(trainingSize).Take(controlSize);
        }
    }
}
