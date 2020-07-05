using Forecaster.Client.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.Config
{
    public class SettingsProvider : IConfigProvider
    {
        public bool IsShowChartPeriod
        {
            get { return Settings.Default.IsShowChartPeriod; }
            set { Settings.Default.IsShowChartPeriod = value; }
        }
        public ushort SelectedAlgorithm
        {
            get { return Settings.Default.SelectedAlgorithm; }
            set { Settings.Default.SelectedAlgorithm = value; }
        }
        public DateTime ScopeStart
        {
            get { return Settings.Default.ScopeStart; }
            set { Settings.Default.ScopeStart = value; }
        }
        public DateTime ScopeEnd
        {
            get { return Settings.Default.ScopeEnd; }
            set { Settings.Default.ScopeEnd = value; }
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
