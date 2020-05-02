using Forecaster.Net;
using Forecaster.Net.Requests;
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
        public ClientWindow()
        {
            InitializeComponent();

            Task task = new Task(() =>
            {
                while (true)
                {
                    ClientModelController.WriteOutput(tbl_output);
                    Thread.Sleep(1000);
                }
            });
            task.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
                tb_fileToUpload.Text = ofd.FileName;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AsynchronousClient client = new AsynchronousClient();

            client.Connect(Dns.GetHostName());

            byte[] fileBytes = File.ReadAllBytes(tb_fileToUpload.Text);

            FileTransferRequest request = new FileTransferRequest(fileBytes);

            RequestManager requestManager = new RequestManager();

            byte[] requestBytes = requestManager.CreateByteRequest(request);

            client.SendData(requestBytes);

        }
    }
}
