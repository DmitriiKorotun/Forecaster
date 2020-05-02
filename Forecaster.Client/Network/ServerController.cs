using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.Network
{
    class test
    {
        public byte[] SendFile(string path)
        {
            byte[] fileBytes = ReadFile(path);

            AsynchronousClient client = new AsynchronousClient();

            client.Connect(Dns.GetHostName());

            client.SendData(fileBytes);

            var result = client.ReceiveResponse();

            return result;
        }

        //private Task<byte[]> Lolec(AsynchronousClient client)
        //{
        //    byte[] responseBytes;

        //    if(client.IsResponseReceived)
        //    {
        //        int size = client.sb
        //    }
        //}

        private byte[] ReadFile(string path)
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
