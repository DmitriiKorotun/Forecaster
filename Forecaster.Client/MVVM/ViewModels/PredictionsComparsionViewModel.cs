using Forecaster.Client.MVVM.Entities;
using Forecaster.Client.MVVM.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forecaster.Client.MVVM.ViewModels
{
    class PredictionsComparsionViewModel : ArgumentfulViewModel
    {
        public ObservableCollection<ComparsionItemControlData> PredictionItems { get; set; }

        public ICommand CloseCommand { get; set; }
        public RelayCommand<ComparsionItemControlData> RemoveItemCommand { get; set; }
        public RelayCommand<ComparsionItemControlData> SaveItemResultCommand { get; set; }

        private List<Dictionary<string, string>> Predictions { get; set; }
        private IOService IoService { get; set; }

        private const string defaultName = "Predictions.txt";

        public PredictionsComparsionViewModel(List<Dictionary<string, string>> predictions)
        {
            Predictions = predictions;

            IEnumerable<ComparsionItemControlData> dataToShow = CreateComparsionItemControlDataRange(Predictions);

            PredictionItems = new ObservableCollection<ComparsionItemControlData>(dataToShow);

            IoService = new Win32IO();

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            CloseCommand = new RelayCommand(OnClosingRequest);

            RemoveItemCommand = new RelayCommand<ComparsionItemControlData>(RemoveItem);
            SaveItemResultCommand = new RelayCommand<ComparsionItemControlData>(SaveItemResult);           
        }

        private IEnumerable<ComparsionItemControlData> CreateComparsionItemControlDataRange(List<Dictionary<string, string>> predictions)
        {
            List<ComparsionItemControlData> itemControlDataList = new List<ComparsionItemControlData>(predictions.Count);

            foreach (Dictionary<string, string> algorithmResult in predictions)
                itemControlDataList.Add(new ComparsionItemControlData() { Predictions = algorithmResult });

            return itemControlDataList;
        }

        private void RemoveItem(object item)
        {
            if (item is ComparsionItemControlData comparsionItem)
                PredictionItems.Remove(comparsionItem);
            else
                throw new ArgumentException();

            if (PredictionItems.Count < 1)
                CloseCommand.Execute(null);
        }

        private void SaveItemResult(object item)
        {
            if (item is ComparsionItemControlData comparsionItem)
            {
                string csvConverted = ConvertToCsv(comparsionItem.Predictions);

                IoService.SaveFileDialog(csvConverted, defaultName);
            }
            else
                throw new ArgumentException();
        }

        protected override void AssignArguments()
        {
            if (Args.Count() > 0)
            {
                if (Args.ElementAt(0) is List<Dictionary<string, string>> predictions)
                    Predictions = predictions;
                else
                    throw new ArgumentException("Error with passed arguments order or no predictions' argument at all");
            }
            else
                throw new ArgumentNullException("Passed empty arguments to view-model");
        }

        private string ConvertToCsv(Dictionary<string, string> predictions)
        {
            string content = "Date,Close\n";

            foreach (KeyValuePair<string, string> prediction in predictions)
                content += prediction.Key.ToString() + ',' + (double.Parse(prediction.Value)).ToString(CultureInfo.InvariantCulture) + '\n';

            return content;
        }
    }
}
