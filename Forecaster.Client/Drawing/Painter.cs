using LiveCharts;
using LiveCharts.Configurations;
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
        public Func<double, string> Formatter { get; set; }

        private CartesianMapper<DateModel> Mapper { get; set; }

        public Painter() 
        {
            Mapper = Mappers.Xy<DateModel>().X(dateModel => dateModel.Date.Ticks / TimeSpan.FromDays(1).Ticks).Y(dateModel => dateModel.Value);

            Series = new SeriesCollection(Mapper);
          
            Formatter = FormatterManager.CreateFormatter();
        }

        public Painter(SeriesCollection series)
        {
            Series = series;

            Mapper = Mappers.Xy<DateModel>().X(dateModel => dateModel.Date.Ticks / TimeSpan.FromDays(1).Ticks).Y(dateModel => dateModel.Value);

            Formatter = FormatterManager.CreateFormatter();
        }

        public void AddLine(LineSeries line)
        {
            Series.Add(line);
        }

        public void UpdateSeries(params Dictionary<DateTime, double>[] csvStockList)
        {
            if (Series == null)
                Series = new DiagrammBuilder().InitSeriesCollection(Mapper, csvStockList);
            else
                SetSeriesLine(csvStockList);
        }

        private void SetSeriesLine(params Dictionary<DateTime, double>[] csvStockList)
        {
            Series.Clear();

            IEnumerable<LineSeries> newLineSeriesRange = new DiagrammBuilder().CreateLineSeriesRange(csvStockList);

            foreach (LineSeries newLineSeries in newLineSeriesRange)
                Series.Add(newLineSeries);
        }
    }
}
