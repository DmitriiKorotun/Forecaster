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
        public ObservableCollection<BasicDataset> Entries { get; set; }

        public ManualInputWindow()
        {
            InitializeComponent();

            Entries = new ObservableCollection<BasicDataset>();

            InitTestEntries();

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgrid_manualData_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (dgrid_manualData.SelectedItem == null)
                return;
        }

        //private void dgrid_manualData_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.LeftCtrl || e.Key == Key.V)
        //    {
        //        var kekov = dgrid_manualData.CurrentCell;

        //        var lol = dgrid_manualData.SelectedItem;

        //        var kek = lol as BasicDataset; 
        //    }
        //}

        private void dgrid_manualData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //var kekov = dgrid_manualData.CurrentCell;

            //var lol = dgrid_manualData.SelectedItem as BasicDataset;

            //lol.Close = decimal.Parse(e.Text);
        }

        private void dgrid_manualData_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IEnumerable<string> gridContent = ReadManualDataGrid();

            IEnumerable<string> csvLines = AddHeadersLine(gridContent);

            byte[] csvBytes = ConvertToByteArray(csvLines);

            OnInputReturn?.Invoke(csvBytes);

            this.DialogResult = true;

            this.Close();
        }

        private IEnumerable<string> ReadManualDataGrid()
        {
            List<string> csvLines = new List<string>(Entries.Count);

            foreach(BasicDataset dataset in Entries)
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

        private void InitTestEntries()
        {
            Random rand = new Random();

            for (int i = 1; i < 31; ++i)
            {
                DateTime date = DateTime.Now.AddDays(i);

                double close = rand.Next(10, 100);

                BasicDataset dataset = new BasicDataset(date, close);

                Entries.Add(dataset);
            }
        }
    }
}
