using Forecaster.Client.Properties;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Drawing
{
    public class Painter : INotifyPropertyChanged
    {
        public SeriesCollection Series { get; set; }
        public Func<double, string> Formatter { get; set; }

        private double minX;
        private double maxX;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double MinX { get { return minX; } set { minX = value; OnPropertyChanged("MinX"); } }
        public double MaxX { get { return maxX; } set { maxX = value; OnPropertyChanged("MaxX"); } }

        private CartesianMapper<DateModel> Mapper { get; set; }

        public Painter() 
        {
            Mapper = Mappers.Xy<DateModel>().X(dateModel => dateModel.Date.Ticks / TimeSpan.FromDays(1).Ticks).Y(dateModel => dateModel.Value);

            Series = new SeriesCollection(Mapper);
          
            Formatter = FormatterManager.CreateFormatter();

            UpdateAxisLimit();
        }

        public Painter(SeriesCollection series)
        {
            Series = series;

            Mapper = Mappers.Xy<DateModel>().X(dateModel => dateModel.Date.Ticks / TimeSpan.FromDays(1).Ticks).Y(dateModel => dateModel.Value);

            Formatter = FormatterManager.CreateFormatter();

            UpdateAxisLimit();
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

            Series.AddRange(newLineSeriesRange);
        }

        public void UpdateAxisLimit()
        {
            if (Settings.Default.IsShowChartPeriod)
            {
                MinX = Settings.Default.ScopeStart.Ticks / TimeSpan.FromDays(1).Ticks;
                MaxX = Settings.Default.ScopeEnd.Ticks / TimeSpan.FromDays(1).Ticks;
            }
            else
            {
                MinX = double.NaN;
                MaxX = double.NaN;
            }
        }
    }
}
