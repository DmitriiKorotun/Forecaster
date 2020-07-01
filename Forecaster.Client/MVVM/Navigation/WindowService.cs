using Forecaster.Client.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Forecaster.Client.MVVM.Navigation
{
    public static class WindowService
    {
        public static void ShowWindow(CloseableViewModel viewModel)
        {
            var win = new Window
            {
                Content = viewModel,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            viewModel.ClosingRequest += (sender, e) => win.Close();

            win.Show();
        }

        public static bool? ShowDialog(CloseableViewModel viewModel)
        {
            var win = new Window
            {
                Content = viewModel,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            viewModel.ClosingRequest += (sender, e) => win.Close();

            return win.ShowDialog();
        }
    }
}
