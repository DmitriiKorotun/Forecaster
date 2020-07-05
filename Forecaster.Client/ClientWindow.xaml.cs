using Csv;
using Forecaster.Client.CSV;
using Forecaster.Client.Drawing;
using Forecaster.Client.Local;
using Forecaster.Client.MVVM.ViewModels;
using Forecaster.Client.Network;
using Forecaster.Client.Properties;
using Forecaster.Net;
using Forecaster.Net.Requests;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forecaster.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private List<Dictionary<string, string>> Predictions { get; set; }

        public ClientWindow()
        {
            InitializeComponent();

            ClientWindowViewModel vm = new ClientWindowViewModel();

            DataContext = vm;

            vm.ClosingRequest += (sender, e) => Close();

            //Task task = new Task(() =>
            //{
            //    while (true)
            //    {
            //        ClientModelController.WriteOutput(tbl_output);
            //        Thread.Sleep(1000);
            //    }
            //});
            //task.Start();
        }
    }
}
