using Csv;
using Forecaster.Client.CSV;
using Forecaster.Client.Drawing;
using Forecaster.Client.IO;
using Forecaster.Client.Local;
using Forecaster.Client.MVVM.Config;
using Forecaster.Client.MVVM.Entities;
using Forecaster.Client.MVVM.IO;
using Forecaster.Client.MVVM.Navigation;
using Forecaster.Client.Network;
using Forecaster.Client.Properties;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Forecaster.Client.MVVM.ViewModels
{
    public enum PredictionAlgorithm : ushort
    {
        MovingAverage = 1,
        LinearRegression = 2,
        KNearest = 4,
        AutoArima = 8,
        LSTM = 128
    }

    class ClientWindowViewModel : CloseableViewModel
    {
        public struct InputGBoxesVisual
        {
            private ushort manualBorderThickness;
            public ushort ManualBorderThickness
            {
                get { return manualBorderThickness; }
                set
                {
                    manualBorderThickness = value;

                    if (value > 0)
                        fileBorderThickness = 0;
                    else
                        fileBorderThickness = 1;
                }
            }

            private ushort fileBorderThickness;
            public ushort FileBorderThickness
            {
                get { return fileBorderThickness; }
                set
                {
                    fileBorderThickness = value;

                    if (value > 0)
                        manualBorderThickness = 0;
                    else
                        manualBorderThickness = 1;
                }
            }
        }

        public class ChartInformation
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public bool IsZoomEnabled { get; set; }

            public ChartInformation(DateTime from, DateTime to, bool isZoomEnabled)
            {
                From = from;
                To = to;
                IsZoomEnabled = isZoomEnabled;
            }

            public override bool Equals(object obj)
            {
                //Check for null and compare run-time types.
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    ChartInformation info = (ChartInformation)obj;
                    return (From == info.From) && (To == info.To) && (IsZoomEnabled == info.IsZoomEnabled);
                }
            }
        }

        public Dictionary<ushort, string> Algorithms { get; set; }

        private ushort selectedAlgorithm;
        public ushort SelectedAlgorithm
        {
            get { return selectedAlgorithm; }
            set
            {
                selectedAlgorithm = value;
                OnPropertyChanged("SelectedAlgorithm");
            }
        }

        private const string defaultPathToStockFile = "C:/Users/korot/source/repos/Forecaster/ForecasterServer/bin/Debug/fortests/NSE-TATAGLOBAL11.csv";
        private string pathToStockFile = defaultPathToStockFile;
        public string PathToStockFile
        {
            get { return pathToStockFile; }
            set
            {
                pathToStockFile = value;
                OnPropertyChanged("PathToStockFile");
            }
        }

        public Painter DiagrammPainter { get; set; }

        public InputGBoxesVisual InputGroupBoxesVisual { get; set; }

        public ICommand OpenManualInputWindowCommand { get; private set; }
        public ICommand OpenSettingsWindowCommand { get; private set; }
        public ICommand OpenPredictionsComparsionWindowCommand { get; private set; }
        public ICommand BuildChartCommand { get; private set; }
        public ICommand ChoseStockFileCommand { get; private set; }
        public ICommand UploadDataCommand { get; private set; }
        public IAsyncCommand LolKek { get; private set; }

        private AsynchronousClient Client { get; set; }

        private List<Dictionary<string, string>> Predictions { get; set; }

        private byte[] StockCsvData { get; set; }

        private bool IsManualSelected { get; set; }

        private bool IsSendButtonDisabled { get; set; } = false;

        private IOService IoService { get; set; }
        private IConfigProvider ConfigProvider { get; set; }


        public ClientWindowViewModel()
        {
            Predictions = new List<Dictionary<string, string>>();

            DiagrammPainter = new Painter();

            IoService = new Win32IO();

            ConfigProvider = new SettingsProvider();

            //ClientController.TransferPredictions += HandlePredictions;

            InitializeAlgorithms();

            InitializeClient();

            InitializeCommands();

            InitializeInputGBoxesVisual();
        }

        private void InitializeAlgorithms()
        {
            Algorithms = new Dictionary<ushort, string> {
                { (ushort)PredictionAlgorithm.MovingAverage, "Moving Average" },
                { (ushort)PredictionAlgorithm.LinearRegression, "Linear Regression" },
                { (ushort)PredictionAlgorithm.KNearest, "KNearestNeighbours" },
                { (ushort)PredictionAlgorithm.AutoArima, "AutoARIMA" }
            };

            SelectedAlgorithm = ConfigProvider.SelectedAlgorithm;
        }

        private void InitializeClient()
        {
            Client = new AsynchronousClient();

            //Client.OnResponseReceived += HandleResponse;
        }

        private void InitializeCommands()
        {
            OpenManualInputWindowCommand = new RelayCommand(OpenManualInputWindow);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindow);
            OpenPredictionsComparsionWindowCommand = new RelayCommand(OpenPredictionsComparsionWindow, (obj) => { return Predictions.Count > 0; });
            BuildChartCommand = new RelayCommand(BuildChart, (obj) => { return !string.IsNullOrEmpty(PathToStockFile); });
            ChoseStockFileCommand = new RelayCommand(ChoseStockFile);
            //UploadDataCommand = new RelayCommand(UploadData, (obj) => { return !IsSendButtonDisabled; });

            LolKek = new AsyncCommand(UploadData, () => { return !IsSendButtonDisabled; });
        }

        private void InitializeInputGBoxesVisual()
        {
            InputGroupBoxesVisual = new InputGBoxesVisual
            {
                FileBorderThickness = 1
            };
        }

        private void OpenManualInputWindow()
        {
            ManualInputViewModel manualInputContext = new ManualInputViewModel();

            WindowService.ShowDialog(manualInputContext);

            if (manualInputContext.DialogResult == true)
            {
                StockCsvData = manualInputContext.ManualCsvBytes;

                IsManualSelected = true;
            }
        }

        private void OpenSettingsWindow()
        {
            SettingsViewModel settingsContext = new SettingsViewModel();

            ChartInformation oldChartInfo = new ChartInformation(settingsContext.ScopeStart, settingsContext.ScopeEnd, settingsContext.IsChartPeriodSelected);

            WindowService.ShowDialog(settingsContext);

            ChartInformation newChartInfo = new ChartInformation(settingsContext.ScopeStart, settingsContext.ScopeEnd, settingsContext.IsChartPeriodSelected);

            if (!oldChartInfo.Equals(newChartInfo))
                DiagrammPainter.UpdateAxisLimit();
        }

        private void OpenPredictionsComparsionWindow()
        {
            if (Predictions.Count < 1)
                MessageBox.Show(Localization.Strings.PredictionsToCompareIsEmpty);
            else
            {
                PredictionsComparsionViewModel predictionsComparsionContext = new PredictionsComparsionViewModel(Predictions);

                WindowService.ShowWindow(predictionsComparsionContext);
            }
        }

        public void BuildChart()
        {
            try
            {
                List<string[]> csvContent = CreateCsvContent();

                if (csvContent != null && csvContent.Count > 0)
                    DrawChart(csvContent);
                else
                    throw new ArgumentNullException("Csv content wasn't initalized or empty");
            }
            catch (ArgumentNullException ex)
            {
                if (ex.Message == Localization.Strings.ManualDataNotEntered)
                    MessageBox.Show(Localization.Strings.ManualDataNotEntered);
                else
                    throw ex;
            }
            catch
            {
                MessageBox.Show(Localization.Strings.DiagrammFillException);
            }
        }

        private List<string[]> CreateCsvContent()
        {
            List<string[]> csvContent;

            if (IsManualSelected)
            {
                if (StockCsvData != null)
                {
                    csvContent = CsvReader.ReadFromBytes(StockCsvData).ToList();
                }
                else
                {
                    throw new ArgumentNullException(Localization.Strings.ManualDataNotEntered);
                }
            }
            else
                csvContent = Reader.ReadCSV(PathToStockFile);

            return csvContent;
        }

        private void DrawChart(List<string[]> csvContent)
        {
            Dictionary<DateTime, double> csvDictionary = CsvConverter.ConvertToDictionary(csvContent);

            Predictions.Clear();

            DiagrammPainter.UpdateSeries(csvDictionary);
        }

        private void ChoseStockFile()
        {
            string selectedPath = IoService.OpenFileDialog(defaultPathToStockFile);

            if (!string.IsNullOrEmpty(selectedPath))
                PathToStockFile = selectedPath;
            else
                IsManualSelected = false;
        }

        private async Task UploadData()
        {
            try
            {
                IsSendButtonDisabled = true;

                using (Client)
                {
                    await SendData();
                    byte[] response = await ReceiveResponseAsync();
                    HandleResponse(response);
                }
            }
            catch (ArgumentNullException ex)
            {
                if (ex.Message == Localization.Strings.ManualDataNotEntered)
                {
                    MessageBox.Show(Localization.Strings.ManualDataNotEntered);
                    IsSendButtonDisabled = false;
                }
                else
                    throw ex;
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == Localization.Strings.EmptyPathFileException)
                {
                    MessageBox.Show(Localization.Strings.EmptyPathFileException);
                    IsSendButtonDisabled = false;
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Localization.Strings.ServerHandleException);
                IsSendButtonDisabled = false;
            }
        }

        private async Task SendData()
        {
            Task sendDataTask;

            if (IsManualSelected)
            {
                if (StockCsvData != null)
                    sendDataTask = ClientController.SendDataAsync(StockCsvData, selectedAlgorithm, Client);
                else
                    throw new ArgumentNullException(Localization.Strings.ManualDataNotEntered);
            }
            else
            {
                if (string.IsNullOrEmpty(PathToStockFile))
                    throw new ArgumentException(Localization.Strings.EmptyPathFileException);

                sendDataTask = ClientController.SendFileAsync(PathToStockFile, selectedAlgorithm, Client);
            }

            await sendDataTask;
        }

        private async Task<byte[]> ReceiveResponseAsync()
        {
            Task<byte[]> receiveDataTask = ClientController.ReceiveResponseAsync(Client);

            return await receiveDataTask;
        }

        private void HandleResponse(byte[] data)
        {
            var restoredPredictions = ClientController.ParseResponse(data).Predictions;

            if (!Predictions.Contains(restoredPredictions))
                Predictions.Add(restoredPredictions);

            DrawPredictions(restoredPredictions);

            IsSendButtonDisabled = false;
        }

        private void DrawPredictions(Dictionary<string, string> restoredPredictions)
        {
            var predictionsToDraw = ResponseConverter.ConvertResponsePredictions(restoredPredictions);

            var lineToDraw = new DiagrammBuilder().CreateLineSeriesRange(predictionsToDraw);

            DiagrammPainter.AddLine(lineToDraw.ElementAt(0));
        }
    }
}