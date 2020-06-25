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

        public delegate void ChartBordersChanged();
        public event ChartBordersChanged OnChartBordersChanged;

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

        private void LoadSettings()
        {
            cb_defaultAlgorithm.ItemsSource = items;
            cb_defaultAlgorithm.SelectedValue = Settings.Default.DefaultAlgorithm;

            LoadRadioState();

            LoadDateBounds();
        }

        private void LoadRadioState()
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

        private void LoadDateBounds()
        {
            calendar_from.SelectedDate = Settings.Default.ScopeStart;
            calendar_to.SelectedDate = Settings.Default.ScopeEnd;
        }

        private void SetDatePickersDisplayRange()
        {
            //calendar_from.DisplayDateEnd = Settings.Default.ScopeEnd.AddDays(-7);
            //calendar_to.DisplayDateStart = Settings.Default.ScopeStart.AddDays(7);
            if (calendar_to.SelectedDate is DateTime && calendar_from.SelectedDate is DateTime)
            {
                DateTime scopeEnd = (DateTime)calendar_to.SelectedDate;
                DateTime scopeStart = (DateTime)calendar_from.SelectedDate;

                if (scopeEnd != null && scopeStart != null)
                {
                    double daysDifference = ((TimeSpan)(calendar_to.SelectedDate - calendar_from.SelectedDate)).TotalDays;

                    if (daysDifference >= 7)
                        AdjustDatePickersDisplayRange(scopeStart, scopeEnd);
                    else
                    {
                        calendar_to.SelectedDate = scopeStart.AddDays(8 - daysDifference);

                        AdjustDatePickersDisplayRange(scopeStart, scopeEnd);
                    }
                }
            }
        }

        private void AdjustDatePickersDisplayRange(DateTime scopeStart, DateTime scopeEnd)
        {
            calendar_from.DisplayDateEnd = scopeEnd.AddDays(-7);

            calendar_to.DisplayDateStart = scopeStart.AddDays(7);
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

        private void SaveSettings()
        {
            try
            {
                Settings.Default.DefaultAlgorithm = Convert.ToUInt16(cb_defaultAlgorithm.SelectedValue);

                if (IsShowOnlyPartOfData)
                    Settings.Default.IsShowPartOfData = true;
                else
                    Settings.Default.IsShowPartOfData = false;

                if (VerifyDateBounds())
                    SetDateBounds();
                else
                    throw new Exception(Localization.Strings.InvalidDateBounds);

                Settings.Default.Save();

                OnChartBordersChanged?.Invoke();
            }
            catch (Exception ex)
            {
                string message;

                if (ex.Message == Localization.Strings.InvalidDateBounds)
                    message = ex.Message;
                else
                    message = Localization.Strings.SaveSettingsError;

                MessageBox.Show(message);
            }
        }

        private void SetDateBounds()
        {
            Settings.Default.ScopeStart = (DateTime)calendar_from.SelectedDate;
            Settings.Default.ScopeEnd = (DateTime)calendar_to.SelectedDate;
        }

        private bool VerifyDateBounds()
        {
            return ((TimeSpan)(calendar_to.SelectedDate - calendar_from.SelectedDate)).TotalDays >= 7 ? true : false;
        }

        private void rb_showAll_Checked(object sender, RoutedEventArgs e)
        {
            IsShowOnlyPartOfData = false;
        }

        private void rb_showPart_Checked(object sender, RoutedEventArgs e)
        {
            IsShowOnlyPartOfData = true;
        }

        private void calendar_from_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDatePickersDisplayRange();
        }

        private void calendar_to_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDatePickersDisplayRange();
        }

        //private void SetScope(DateTime scope, DateTime datePickerValue)
        //{
        //    scope = datePickerValue;

        //    AdjustDatePickersRange();
        //}
    }
}
