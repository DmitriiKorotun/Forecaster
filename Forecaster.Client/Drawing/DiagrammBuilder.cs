using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Forecaster.Client.Drawing
{
    class DiagrammBuilder
    {
        public SeriesCollection InitSeriesCollection(CartesianMapper<DateModel> mapper, params Dictionary<DateTime, double>[] dataSeries)
        {
            SeriesCollection seriesCollection = new SeriesCollection(mapper);

            foreach (Dictionary<DateTime, double> diagrammData in dataSeries)
            {
                LineSeries lineSeries = CreateLineSeries(diagrammData);

                seriesCollection.Add(lineSeries);
            }

            return seriesCollection;
        }

        public IEnumerable<LineSeries> CreateLineSeriesRange(params Dictionary<DateTime, double>[] dataSeries)
        {
            List<LineSeries> lineSeriesList = new List<LineSeries>(dataSeries.Length);

            foreach (Dictionary<DateTime, double> diagrammData in dataSeries)
            {
                LineSeries lineSeries = CreateLineSeries(diagrammData);

                lineSeriesList.Add(lineSeries);
            }

            return lineSeriesList;
        }

        private LineSeries CreateLineSeries(Dictionary<DateTime, double> diagrammData)
        {
            return new LineSeries
            {
                Values = ConvertToDateModel(diagrammData),
                PointGeometry = null,
                Fill = Brushes.Transparent
            };
        }

        private ChartValues<DateModel> ConvertToDateModel(Dictionary<DateTime, double> dateDecimalDictionary)
        {
            var values = new ChartValues<DateModel>();

            foreach (KeyValuePair<DateTime, double> dateDecimalPair in dateDecimalDictionary)
            {
                var dateModel = new DateModel
                {
                    Date = dateDecimalPair.Key,
                    Value = dateDecimalPair.Value
                };

                values.Add(dateModel);
            }

            return values;
        }
    }
}
