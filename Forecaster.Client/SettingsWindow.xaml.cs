using Forecaster.Client.Properties;
using System;
using System.Collections.Generic;
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
using static Forecaster.Client.ClientWindow;

namespace Forecaster.Client
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public bool IsShowOnlyPartOfData = Settings.Default.IsShowPartOfData;

        public DateTime ScopeStart { get; set; } = Settings.Default.ScopeStart;
        public DateTime ScopeEnd { get; set; } = Settings.Default.ScopeEnd;

        readonly Dictionary<ushort, string> items = new Dictionary<ushort, string> {
                { (ushort)PredictionAlgorithm.MovingAverage, "Moving Average" },
                { (ushort)PredictionAlgorithm.LinearRegression, "Linear Regression" },
                { (ushort)PredictionAlgorithm.KNearest, "KNearestNeighbours" },
                { (ushort)PredictionAlgorithm.AutoArima, "AutoARIMA" }
            };

        public SettingsWindow()
        {
            InitializeComponent();

            LoadSettings();

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            DialogResult = true;

            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //cb_defaultAlgorithm.SelectedItem = "Moving Average";
            DialogResult = false;

            this.Close();
        }

        private void LoadSettings()
        {
            cb_defaultAlgorithm.ItemsSource = items;
            cb_defaultAlgorithm.SelectedValue = Settings.Default.DefaultAlgorithm;

            SetRadioState();
        }

        private void SaveSettings()
        {
            Settings.Default.DefaultAlgorithm = Convert.ToUInt16(cb_defaultAlgorithm.SelectedValue);

            if (IsShowOnlyPartOfData)
                Settings.Default.IsShowPartOfData = true;
            else
                Settings.Default.IsShowPartOfData = false;

            Settings.Default.ScopeStart = ScopeStart;
            Settings.Default.ScopeEnd = ScopeEnd;

            Settings.Default.Save();
        }

        private void SetRadioState()
        {
            if (IsShowOnlyPartOfData)
            {
                rb_showPart.IsChecked = true;
                rb_showAll.IsChecked = false;
            }
            else
            {
                rb_showPart.IsChecked = false;
                rb_showAll.IsChecked = true;
            }
        }

        private void rb_showAll_Checked(object sender, RoutedEventArgs e)
        {
            IsShowOnlyPartOfData = false;
        }

        private void rb_showPart_Checked(object sender, RoutedEventArgs e)
        {
            IsShowOnlyPartOfData = true;
        }
    }
}
