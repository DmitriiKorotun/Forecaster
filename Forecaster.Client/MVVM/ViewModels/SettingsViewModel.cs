using Forecaster.Client.MVVM.Config;
using Forecaster.Client.MVVM.Entities;
using Forecaster.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Forecaster.Client.MVVM.ViewModels
{
    class SettingsViewModel : CloseableViewModel
    {
        public IConfigProvider ConfigProvider { get; set; }

        public bool IsAllChartSelected
        {
            get { return !ConfigProvider.IsShowChartPeriod; }
            set
            {
                ConfigProvider.IsShowChartPeriod = !value;
                OnPropertyChanged("IsAllChartSelected");
            }
        }

        public bool IsChartPeriodSelected
        {
            get { return ConfigProvider.IsShowChartPeriod; }
            set
            {
                ConfigProvider.IsShowChartPeriod = value;
                OnPropertyChanged("IsChartPeriodSelected");
            }
        }

        public DateTime ScopeStart
        {
            get { return ConfigProvider.ScopeStart; }
            set
            {
                ConfigProvider.ScopeStart = value;
                OnPropertyChanged("ScopeStart");
            }
        }

        public DateTime ScopeEnd
        {
            get { return ConfigProvider.ScopeEnd; }
            set
            {
                ConfigProvider.ScopeEnd = value;
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
            get { return ConfigProvider.SelectedAlgorithm; }
            set
            {
                ConfigProvider.SelectedAlgorithm = value;
                OnPropertyChanged("SelectedAlgorithm");
            }
        }

        public Dictionary<ushort, string> Algorithms { get; } = new Dictionary<ushort, string> {
                { (ushort)PredictionAlgorithm.MovingAverage, "Moving Average" },
                { (ushort)PredictionAlgorithm.LinearRegression, "Linear Regression" },
                { (ushort)PredictionAlgorithm.KNearest, "KNearestNeighbours" },
                { (ushort)PredictionAlgorithm.AutoArima, "AutoARIMA" }
            };

        public ICommand CancelCommand { get; private set; }
        public ICommand ApplyCommand { get; private set; }

        public SettingsViewModel()
        {
            InitCommands();

            ConfigProvider = new SettingsProvider();
        }

        private void InitCommands()
        {
            ApplyCommand = new RelayCommand(Apply, (obj) => { return CheckDatesBound(); });
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Apply()
        {
            ConfigProvider.Save();

            OnClosingRequest();
        }

        private void Cancel()
        {
            OnClosingRequest();
        }

        private bool CheckDatesBound()
        {
            return (ScopeEnd - ScopeStart).Days >= 7;
        }
    }
}
