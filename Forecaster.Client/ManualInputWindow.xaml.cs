using Forecaster.Client.MVVM.ViewModels;
using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Forecaster.Client
{
    /// <summary>
    /// Логика взаимодействия для ManualInputWindow.xaml
    /// </summary>
    public partial class ManualInputWindow : Window
    {
        public event Action<byte[]> OnInputReturn;

        public ManualInputWindow()
        {
            InitializeComponent();

            CloseableViewModel vm = new ManualInputViewModel();

            DataContext = vm;

            vm.ClosingRequest += (sender, e) => Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_apply_Click(object sender, RoutedEventArgs e)
        {
            //IEnumerable<string> gridContent = ReadManualDataGrid();

            //IEnumerable<string> csvLines = AddHeadersLine(gridContent);

            //byte[] csvBytes = ConvertToByteArray(csvLines);

            //OnInputReturn?.Invoke(csvBytes);

            //DialogResult = true;

            //Close();
        }

        //private IEnumerable<string> ReadManualDataGrid()
        //{
        //    List<string> csvLines = new List<string>(Entries.Count);

        //    foreach(BasicDataset dataset in Entries)
        //    {
        //        string csvLine = dataset.ToString() + '\n';

        //        csvLines.Add(csvLine);
        //    }

        //    return csvLines;
        //}

        private IEnumerable<string> AddHeadersLine(IEnumerable<string> csvLines)
        {
            var csvLinesList = csvLines.ToList();

            csvLinesList.Insert(0, "Date,Close\n");

            return csvLinesList;
        }

        private byte[] ConvertToByteArray(IEnumerable<string> csvLines)
        {
            List<byte[]> csvBytes = new List<byte[]>(csvLines.Count());

            foreach (string csvLine in csvLines)
            {
                byte[] csvLineBytes = Encoding.UTF8.GetBytes(csvLine);

                csvBytes.Add(csvLineBytes);
            }

            return csvBytes.SelectMany(a => a).ToArray();
        }

        
    }
}
