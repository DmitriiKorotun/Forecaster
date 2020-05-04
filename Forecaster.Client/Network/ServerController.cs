using Forecaster.Net;
using Forecaster.Net.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Forecaster.Client.Network
{
    public static class ServerController
    {
        public static byte[] SendFile(string path, ushort selectedAlgortihm, ClientWindow window)
        {
            try
            {
                byte[] fileBytes = ReadFile(path),
                    requestBytes = CreateFTRequestBytes(fileBytes, selectedAlgortihm);

                using (AsynchronousClient client = new AsynchronousClient())
                {
                    //client.ExceptionReport += (sender, e) =>
                    //{
                    //    window.Dispatcher.BeginInvoke((MethodInvoker)(() =>
                    //        MessageBox.Show(e.Exception.Message, Application.ProductName,
                    //            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    //};

                    client.Connect(Dns.GetHostName());

                    client.SendData(requestBytes);

                    var result = client.ReceiveResponse();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ParseResponse()
        {

        }

        private static byte[] CreateFTRequestBytes(byte[] fileBytes, ushort selectedAlgorithm)
        {
            FileTransferRequest request = new FileTransferRequest(fileBytes, selectedAlgorithm);

            return GetFTRequestBytes(request);
        }

        private static byte[] GetFTRequestBytes(FileTransferRequest request)
        {
            RequestManager requestManager = new RequestManager();

            return requestManager.CreateByteRequest(request);
        }

        private static byte[] ReadFile(string path)
        {
            byte[] fileBytes;

            if (File.Exists(path))
            {
                try
                {
                    fileBytes = File.ReadAllBytes(path);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            else
                throw new FileNotFoundException("File wasn't found");

            return fileBytes;
        }
    }
}
