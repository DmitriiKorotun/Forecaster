using Forecaster.Client.MVVM.ViewModels;
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

namespace Forecaster.Client
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public delegate void ChartBordersChanged();
        public event ChartBordersChanged OnChartBordersChanged;

        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = new SettingsViewModel();
        }

        private void btn_apply_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            DialogResult = true;

            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void SaveSettings()
        {
            try
            {
                if (VerifyDateBounds())
                {
                    Settings.Default.Save();

                    OnChartBordersChanged?.Invoke();
                }
                else
                    throw new Exception(Localization.Strings.InvalidDateBounds);
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

        private bool VerifyDateBounds()
        {
            return ((TimeSpan)(calendar_to.SelectedDate - calendar_from.SelectedDate)).TotalDays >= 7 ? true : false;
        }
    }
}
