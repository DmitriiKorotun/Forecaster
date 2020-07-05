using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void SaveFileDialog(string content, string defaultName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = defaultName
            };

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, content);
        }
    }
}
