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
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection Series { get; set; }

        private AsynchronousClient Client { get; set; }
        public Painter DiagrammPainter { get; set; }

        private byte[] DataToPredict { get; set; }

        private bool IsManualSelected { get; set; }

        private List<Dictionary<string, string>> Predictions { get; set; }

        public ClientWindow()
        {
            InitializeComponent();

            ClientWindowViewModel vm = new ClientWindowViewModel();

            DataContext = vm;

            vm.ClosingRequest += (sender, e) => Close();

            InitializeClient();

            //Task task = new Task(() =>
            //{
            //    while (true)
            //    {
            //        ClientModelController.WriteOutput(tbl_output);
            //        Thread.Sleep(1000);
            //    }
            //});
            //task.Start();

            ClientController.TransferPredictions += HandlePredictions;
        }

        private void InitializeClient()
        {
            Client = new AsynchronousClient();

            Client.Transfer += ClientController.HandleResponse;
        }

        private void HandlePredictions(Dictionary<string, string> restoredPredictions)
        {
            if(!Predictions.Contains(restoredPredictions))
                Predictions.Add(restoredPredictions);

            DrawPredictions(restoredPredictions);
        }

        private void DrawPredictions(Dictionary<string, string> restoredPredictions)
        {
            var predictionsToDraw = ResponseConverter.ConvertResponsePredictions(restoredPredictions);

            var lineToDraw = new DiagrammBuilder().CreateLineSeriesRange(predictionsToDraw);

            DiagrammPainter.AddLine(lineToDraw.ElementAt(0));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ushort selectedAlgorithm = (ushort)cb_algList.SelectedValue;

                if (IsManualSelected)
                {
                    if (DataToPredict != null)
                    {
                        ClientController.SendFile(DataToPredict, selectedAlgorithm, Client);
                    }
                    else
                        MessageBox.Show(Localization.Strings.ManualDataNotEntered);
                }
                else
                {
                    if (string.IsNullOrEmpty(tb_fileToUpload.Text))
                        throw new ArgumentException(Localization.Strings.EmptyPathFileException);

                    ClientController.SendFile(tb_fileToUpload.Text, selectedAlgorithm, Client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Localization.Strings.ServerHandleException);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (Predictions.Count < 1)
                MessageBox.Show(Localization.Strings.PredictionsToCompareIsEmpty);
            else
            {
                ResultsWindow resultsWindow = new ResultsWindow(Predictions);

                resultsWindow.ShowDialog();
            }
        }
    }
}
