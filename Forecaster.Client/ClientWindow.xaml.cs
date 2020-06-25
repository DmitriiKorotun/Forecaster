using Csv;
using Forecaster.Client.CSV;
using Forecaster.Client.Drawing;
using Forecaster.Client.Local;
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
        public enum PredictionAlgorithm : ushort
        {
            MovingAverage = 1,
            LinearRegression = 2,
            KNearest = 4,
            AutoArima = 8,
            LSTM = 128
        }

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

            InitializeClient();

            Task task = new Task(() =>
            {
                while (true)
                {
                    ClientModelController.WriteOutput(tbl_output);
                    Thread.Sleep(1000);
                }
            });
            task.Start();

            InitAlgorithmsCB();

            Formatter = FormatterManager.CreateFormatter();

            DiagrammPainter = new Painter();

            DataContext = this;

            ClientController.TransferPredictions += HandlePredictions;

            Predictions = new List<Dictionary<string, string>>();
        }

        private void InitializeClient()
        {
            Client = new AsynchronousClient();

            Client.Transfer += ClientController.HandleResponse;
        }

        private void ReceiveResponse(byte[] data)
        {
            InitAlgorithmsCB();
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

        private void InitAlgorithmsCB()
        {
            Dictionary<ushort, string> items = new Dictionary<ushort, string> {
                { (ushort)PredictionAlgorithm.MovingAverage, "Moving Average" },
                { (ushort)PredictionAlgorithm.LinearRegression, "Linear Regression" },
                { (ushort)PredictionAlgorithm.KNearest, "KNearestNeighbours" },
                { (ushort)PredictionAlgorithm.AutoArima, "AutoARIMA" }
            };
            cb_algList.ItemsSource = items;
            cb_algList.SelectedValue = (ushort)Settings.Default.DefaultAlgorithm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
                tb_fileToUpload.Text = ofd.FileName;

            IsManualSelected = false;

            ChangeInputGBoxVisual();
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

        private void btn_fillDiagramm_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> csvContent;

            try
            {
                if (IsManualSelected)
                {
                    if (DataToPredict != null)
                    {
                        csvContent = CsvReader.ReadFromBytes(DataToPredict).ToList();
                    }
                    else
                    {
                        MessageBox.Show(Localization.Strings.ManualDataNotEntered);

                        return;
                    }
                }
                else
                    csvContent = Reader.ReadCSV(tb_fileToUpload.Text);

                Dictionary<DateTime, double> csvDictionary = CsvConverter.ConvertToDictionary(csvContent);

                Predictions.Clear();

                DiagrammPainter.UpdateSeries(csvDictionary);
            }
            catch
            {
                MessageBox.Show(Localization.Strings.DiagrammFillException);
            }
        }

        private void SetSeriesLine(params Dictionary<DateTime, double>[] csvStockList)
        {
            Series.Clear();

            IEnumerable<LineSeries> newLineSeriesRange = new DiagrammBuilder().CreateLineSeriesRange(csvStockList);

            foreach (LineSeries newLineSeries in newLineSeriesRange)
                Series.Add(newLineSeries);
        }

        private void SetDataContext()
        {
            if (DataContext == null)
                DataContext = this;
        }

        private void btn_manualInput_Click(object sender, RoutedEventArgs e)
        {
            ManualInputWindow manualInputWindow = new ManualInputWindow();

            manualInputWindow.OnInputReturn += SetManualData;

            if (manualInputWindow.ShowDialog() == true) 
            {
                IsManualSelected = true;

                ChangeInputGBoxVisual();
            }        
        }

        private void SetManualData(byte[] arr)
        {
            DataToPredict = arr;
        }

        private void ChangeInputGBoxVisual()
        {
            if (IsManualSelected)
            {
                ChangeGroupBoxBorder(gbox_manualInput, true);
                ChangeGroupBoxBorder(gbox_choseFile, false);
            }
            else
            {
                ChangeGroupBoxBorder(gbox_manualInput, false);
                ChangeGroupBoxBorder(gbox_choseFile, true);
            }
        }

        private void ChangeGroupBoxBorder(GroupBox gbox, bool isSelected)
        {
            if (isSelected)
            {
                gbox.BorderBrush = new SolidColorBrush(Colors.Black);

                gbox.BorderThickness = new Thickness(1);
            }
            else
                gbox.BorderThickness = new Thickness(0);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();

            settingsWindow.OnChartBordersChanged += DiagrammPainter.SetAxisLimit;

            settingsWindow.ShowDialog();
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
