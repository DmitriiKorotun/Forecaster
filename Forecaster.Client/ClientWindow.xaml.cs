using Forecaster.Client.CSV;
using Forecaster.Client.Drawing;
using Forecaster.Client.Local;
using Forecaster.Client.Network;
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

            ClientController.TransferPredictions += Test;
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

        private void Test(Dictionary<string, string> restoredPredictions)
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
            cb_algList.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
                tb_fileToUpload.Text = ofd.FileName;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ushort selectedAlgorithm = (ushort)cb_algList.SelectedValue;

                if (DataToPredict != null)
                {
                    ClientController.SendFile(DataToPredict, selectedAlgorithm, Client);
                }
                else
                {
                    if (string.IsNullOrEmpty(tb_fileToUpload.Text))
                        throw new ArgumentException("Path to file can not be empty");

                    ClientController.SendFile(tb_fileToUpload.Text, selectedAlgorithm, Client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //AsynchronousClient client = new AsynchronousClient();

            //client.Connect(Dns.GetHostName());

            //byte[] fileBytes = File.ReadAllBytes(tb_fileToUpload.Text);

            //FileTransferRequest request = new FileTransferRequest(fileBytes);

            //RequestManager requestManager = new RequestManager();

            //byte[] requestBytes = requestManager.CreateByteRequest(request);

            //client.SendData(requestBytes);

        }

        private void btn_fillDiagramm_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> csvContent = Reader.ReadCSV(tb_fileToUpload.Text);

            Dictionary<DateTime, double> csvDictionary = CsvConverter.ConvertToDictionary(csvContent);

            DiagrammPainter.UpdateSeries(csvDictionary);

            //SetDataContext();
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

            manualInputWindow.OnInputReturn += Kekovuce;

            manualInputWindow.ShowDialog();
        }

        private void Kekovuce(byte[] arr)
        {
            DataToPredict = arr;
        }
    }
}
