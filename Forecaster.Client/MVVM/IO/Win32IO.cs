using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.MVVM.IO
{
    public class Win32IO : IOService
    {
        public string OpenFileDialog(string defaultPath)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                FileName = defaultPath
            };

            return ofd.ShowDialog() == true ? ofd.FileName : string.Empty;
        }
    }
}
