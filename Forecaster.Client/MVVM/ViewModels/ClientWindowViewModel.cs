using Csv;
using Forecaster.Client.CSV;
using Forecaster.Client.Drawing;
using Forecaster.Client.Local;
using Forecaster.Client.MVVM.Entities;
using Forecaster.Client.MVVM.IO;
using Forecaster.Client.MVVM.Navigation;
using Forecaster.Client.Network;
using Forecaster.Client.Properties;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        }

        private ChartInformation ChartInfo { get; set; }

        private AsynchronousClient Client { get; set; }

        public Painter DiagrammPainter { get; set; }

        public InputGBoxesVisual InputGroupBoxesVisual { get; set; }

        private byte[] StockCsvData { get; set; }

        private bool IsManualSelected { get; set; }

        private List<Dictionary<string, string>> Predictions { get; set; }

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

        private IOService IoService { get; set; }
     
        public ICommand OpenManualInputWindowCommand { get; private set; }
        public ICommand OpenSettingsWindowCommand { get; private set; }
        public ICommand OpenPredictionsComparsionWindowCommand { get; private set; }
        public ICommand BuildChartCommand { get; private set; }
        public ICommand ChoseStockFileCommand { get; private set; }
        public ICommand UploadDataCommand { get; private set; }

        public void BuildChart()
        {
            List<string[]> csvContent;

            try
            {
                if (IsManualSelected)
                {
                    if (StockCsvData != null)
                    {
                        csvContent = CsvReader.ReadFromBytes(StockCsvData).ToList();
                    }
                    else
                    {
                        MessageBox.Show(Localization.Strings.ManualDataNotEntered);

                        return;
                    }
                }
                else
                    csvContent = Reader.ReadCSV(PathToStockFile);

                Dictionary<DateTime, double> csvDictionary = CsvConverter.ConvertToDictionary(csvContent);

                Predictions.Clear();

                DiagrammPainter.UpdateSeries(csvDictionary);
            }
            catch
            {
                MessageBox.Show(Localization.Strings.DiagrammFillException);
            }
        }

        public ClientWindowViewModel()
        {
            Predictions = new List<Dictionary<string, string>>();

            DiagrammPainter = new Painter();

            IoService = new Win32IO();

            ClientController.TransferPredictions += HandlePredictions;

            InitAlgorithms();

            InitializeClient();

            InitializeCommands();

            InitInputGBoxesVisual();          
        }

        private void InitializeCommands()
        {           
            OpenManualInputWindowCommand = new RelayCommand(OpenManualInputWindow);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindow);
            OpenPredictionsComparsionWindowCommand = new RelayCommand(OpenPredictionsComparsionWindow);
            BuildChartCommand = new RelayCommand(BuildChart);
            ChoseStockFileCommand = new RelayCommand(ChoseStockFile);
            UploadDataCommand = new RelayCommand(UploadData);
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

            //ChartInfo = new ChartInformation(settingsContext.ScopeStart, settingsContext.ScopeEnd, settingsContext.IsChartPeriodSelected);

            WindowService.ShowDialog(settingsContext);

            DiagrammPainter.UpdateAxisLimit();
        }

        private void InitAlgorithms()
        {
            Algorithms = new Dictionary<ushort, string> {
                { (ushort)PredictionAlgorithm.MovingAverage, "Moving Average" },
                { (ushort)PredictionAlgorithm.LinearRegression, "Linear Regression" },
                { (ushort)PredictionAlgorithm.KNearest, "KNearestNeighbours" },
                { (ushort)PredictionAlgorithm.AutoArima, "AutoARIMA" }
            };

            SelectedAlgorithm = Settings.Default.SelectedAlgorithm;
        }

        private void InitInputGBoxesVisual()
        {
            InputGroupBoxesVisual = new InputGBoxesVisual
            {
                FileBorderThickness = 1
            };
        }

        private void InitializeClient()
        {
            Client = new AsynchronousClient();

            Client.Transfer += ClientController.HandleResponse;
        }

        private void ChoseStockFile()
        {
            string selectedPath = IoService.OpenFileDialog(defaultPathToStockFile);

            if (!string.IsNullOrEmpty(selectedPath))
            {
                PathToStockFile = selectedPath;
            }
            else
                IsManualSelected = false;
        }

        private void UploadData()
        {
            try
            {
                if (IsManualSelected)
                {
                    if (StockCsvData != null)
                    {
                        ClientController.SendFile(StockCsvData, selectedAlgorithm, Client);
                    }
                    else
                        MessageBox.Show(Localization.Strings.ManualDataNotEntered);
                }
                else
                {
                    if (string.IsNullOrEmpty(PathToStockFile))
                        throw new ArgumentException(Localization.Strings.EmptyPathFileException);

                    ClientController.SendFile(PathToStockFile, selectedAlgorithm, Client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Localization.Strings.ServerHandleException);
            }
        }

        private void HandlePredictions(Dictionary<string, string> restoredPredictions)
        {
            if (!Predictions.Contains(restoredPredictions))
                Predictions.Add(restoredPredictions);

            DrawPredictions(restoredPredictions);
        }

        private void DrawPredictions(Dictionary<string, string> restoredPredictions)
        {
            var predictionsToDraw = ResponseConverter.ConvertResponsePredictions(restoredPredictions);

            var lineToDraw = new DiagrammBuilder().CreateLineSeriesRange(predictionsToDraw);

            DiagrammPainter.AddLine(lineToDraw.ElementAt(0));
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
    }
}
