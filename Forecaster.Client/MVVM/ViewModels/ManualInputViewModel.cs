using Forecaster.Client.MVVM.Entities;
using Forecaster.Client.MVVM.ViewModels;
using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forecaster.Client.MVVM.ViewModels
{
    class ManualInputViewModel : CloseableViewModel
    {
        public bool? DialogResult { get; set; }
        public ObservableCollection<BasicDataset> Entries { get; set; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public byte[] ManualCsvBytes { get; private set; }

        public ManualInputViewModel()
        {
            Entries = new ObservableCollection<BasicDataset>();

            //InitTestEntries();

            ApplyCommand = new RelayCommand(Apply, (obj) => { return Entries.Count > 0; });
            CloseCommand = new RelayCommand(OnClosingRequest);
        }

        private void InitTestEntries()
        {
            Random rand = new Random();

            int seasonCount = 20, valuePerSeason = 15, maxValueChangePerDay = 5, maxValueChangeJumped = 10, lastSeasonValue = 50;

            double enoughJumpProbability = 0.8;

            bool isGrowExpected;

            for (int i = 0; i < seasonCount; ++i)
            {
                isGrowExpected = rand.Next(0, 2) > 0 ? true : false;

                for (int j = 1; j < valuePerSeason + 1; ++j)
                {
                    DateTime date = DateTime.Now.AddDays(i * seasonCount + j);

                    double close, jumpProbability = rand.NextDouble();

                    if (isGrowExpected)
                    {
                        if (jumpProbability > enoughJumpProbability)
                            close = rand.Next(lastSeasonValue, lastSeasonValue + maxValueChangeJumped);
                        else
                            close = rand.Next(lastSeasonValue, lastSeasonValue + maxValueChangePerDay);
                    }
                    else
                    {
                        int minClose;

                        if (jumpProbability > enoughJumpProbability)
                            minClose = lastSeasonValue - maxValueChangePerDay;
                        else
                            minClose = lastSeasonValue - maxValueChangeJumped;

                        if (minClose < 1)
                            minClose = 1;

                        close = rand.Next(minClose, lastSeasonValue);
                    }

                    BasicDataset dataset = new BasicDataset(date, close);

                    Entries.Add(dataset);
                }

                lastSeasonValue = (int)Entries[Entries.Count - 1].Close;
            }
        }

        private IEnumerable<string> ReadManualDataGrid()
        {
            List<string> csvLines = new List<string>(Entries.Count);

            foreach (BasicDataset dataset in Entries)
            {
                string csvLine = dataset.ToString() + '\n';

                csvLines.Add(csvLine);
            }

            return csvLines;
        }

        private IEnumerable<string> AddHeadersLine(IEnumerable<string> csvLines)
        {
            var csvLinesList = csvLines.ToList();

            csvLinesList.Insert(0, "Date,Close\n");

            return csvLinesList;
        }

        private void Apply()
        {
            if (Entries.Count >= 20)
            {
                IEnumerable<string> gridContent = ReadManualDataGrid();

                IEnumerable<string> csvLines = AddHeadersLine(gridContent);

                ManualCsvBytes = ConvertToByteArray(csvLines);

                DialogResult = true;

                OnClosingRequest();
            }
            else
                MessageBox.Show(Localization.Strings.NeedMoreDataToPredict);
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
