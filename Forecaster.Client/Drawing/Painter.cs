using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Drawing
{
    public class Painter
    {
        public SeriesCollection Series { get; set; }

        public void AddLineSeries(params Dictionary<DateTime, double>[] csvStockList)
        {
            IEnumerable<LineSeries> newLineSeriesRange = new DiagrammBuilder().CreateLineSeriesRange(csvStockList);

            foreach (LineSeries newLineSeries in newLineSeriesRange)
                Series.Add(newLineSeries);
        }
    }
}
