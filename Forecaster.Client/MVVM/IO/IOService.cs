using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.IO
{
    interface IOService
    {
        string OpenFileDialog(string defaultPath);
        void SaveFileDialog(string content, string defaultName);
    }
}
