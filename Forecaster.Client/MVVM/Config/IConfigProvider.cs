using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.Config
{
    interface IConfigProvider
    {  
        bool IsShowChartPeriod { get; set; }
        ushort SelectedAlgorithm { get; set; }
        DateTime ScopeStart { get; set; }
        DateTime ScopeEnd { get; set; }

        void Save();
    }
}
