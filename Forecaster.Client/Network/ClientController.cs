using Forecaster.Client.IO;
using Forecaster.Net;
using Forecaster.Net.Requests;
using Forecaster.Net.Responses;
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
    public static class ClientController
    {
        private static IReader Reader { get; set; } = new SystemReader();

        public static void SendFile(string path, ushort selectedAlgortihm, AsynchronousClient client)
        {
            byte[] fileBytes;

            try
            {
                fileBytes = Reader.ReadAllBytes(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            SendData(fileBytes, selectedAlgortihm, client);
        }

        public static async Task SendFileAsync(string path, ushort selectedAlgortihm, AsynchronousClient client)
        {
            Task<byte[]> readFileTask = ReadFileAsync(path);

            byte[] fileBytes = await readFileTask.ConfigureAwait(false);

            await SendDataAsync(fileBytes, selectedAlgortihm, client).ConfigureAwait(false);
        }

        private static async Task<byte[]> ReadFileAsync(string path)
        {
            return await Task.Run(() =>
            {
                byte[] fileBytes;

                try
                {
                    fileBytes = Reader.ReadAllBytes(path);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return fileBytes;
            }).ConfigureAwait(false);
        }

        public static void SendData(byte[] dataToPredict, ushort selectedAlgortihm, AsynchronousClient client)
        {
            try
            {
                byte[] requestBytes = CreateFTRequestBytes(dataToPredict, selectedAlgortihm);

                client.ExceptionReport += (sender, e) =>
                {
                    client.Dispose();
                };

                client.Connect(Dns.GetHostName());

                client.SendData(requestBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task SendDataAsync(byte[] dataToPredict, ushort selectedAlgortihm, AsynchronousClient client)
        {
            await Task.Run(() => SendData(dataToPredict, selectedAlgortihm, client)).ConfigureAwait(false);
        }

        public static byte[] ReceiveResponse(AsynchronousClient client)
        {
            return client.ReceiveResponse();
        }

        public static async Task<byte[]> ReceiveResponseAsync(AsynchronousClient client)
        {
            return await Task.Run(() => ReceiveResponse(client)).ConfigureAwait(false);
        }

        public static PredictionResponse ParseResponse(byte[] responseBytes)
        {
            Response basicResponse = ResponseHandler.RestoreResponse<Response>(responseBytes);

            PredictionResponse response;

            if (basicResponse.ResponseCode == (int)ResponseCode.OK)
                response = ResponseHandler.RestoreResponse<PredictionResponse>(responseBytes);
            else
                throw new Exception("Server sent a response with " + basicResponse.ResponseCode.ToString() + " error code");

            return response;
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
    }
}
