using Forecaster.Net;
using Forecaster.Net.Helpers;
using Forecaster.Net.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.Network
{
    public static class RequestHandler
    {
        public static T RestoreRequest<T>(byte[] data) where T : Request
        {
            int requestLength = ReadRequestLength(data);

            byte[] requestBytes = data.Skip(sizeof(int)).Take(requestLength).ToArray();

            T request = new RequestManager().RestoreFromBytes<T>(requestBytes);

            return request;
        }

        public static void ReceiveFile()
        {

        }

        public static int ReadRequestLength(byte[] data)
        {
            byte[] sizeBytes = data.Take(4).ToArray();

            return ByteConverter.ReadIntFromBytes(sizeBytes);
        }
    }
}
