using Forecaster.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Forecaster.Client.ClientWindow;

namespace Forecaster.Client.ViewModels
{
    class SettingsViewModel : BasicViewModel
    {
        public bool IsAllChartSelected
        {
            get { return !Settings.Default.IsShowChartPeriod; }
            set
            {
                Settings.Default.IsShowChartPeriod = !value;
                OnPropertyChanged("IsAllChartSelected");
            }
        }

        public bool IsChartPeriodSelected
        {
            get { return Settings.Default.IsShowChartPeriod; }
            set
            {
                Settings.Default.IsShowChartPeriod = value;
                OnPropertyChanged("IsChartPeriodSelected");
            }
        }

        public DateTime ScopeStart
        {
            get { return Settings.Default.ScopeStart; }
            set
            {
                Settings.Default.ScopeStart = value;
                OnPropertyChanged("ScopeStart");
            }
        }

        public DateTime ScopeEnd
        {
            get { return Settings.Default.ScopeEnd; }
            set
            {
                Settings.Default.ScopeEnd = value;
                OnPropertyChanged("ScopeEnd");
            }
        }

        public DateTime DisplayDateEndFrom
        {
            get { return ScopeEnd.AddDays(-7); }
        }

        public DateTime DisplayDateStartTo
        {
            get { return ScopeStart.AddDays(7); }
        }

        public ushort SelectedAlgorithm
        {
            get { return Settings.Default.SelectedAlgorithm; }
            set
            {
                Settings.Default.SelectedAlgorithm = value;
                OnPropertyChanged("SelectedAlgorithm");
            }
        }

        public Dictionary<ushort, string> Items { get; set; } = new Dictionary<ushort, string> {
                { (ushort)PredictionAlgorithm.MovingAverage, "Moving Average" },
                { (ushort)PredictionAlgorithm.LinearRegression, "Linear Regression" },
                { (ushort)PredictionAlgorithm.KNearest, "KNearestNeighbours" },
                { (ushort)PredictionAlgorithm.AutoArima, "AutoARIMA" }
            };
    }
}
