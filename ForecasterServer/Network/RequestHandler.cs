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

            // First 4 bytes allocated for received data size, so we shall skip it
            byte[] requestBytes = data.Skip(sizeof(int)).Take(requestLength).ToArray();

            T request = new RequestManager().RestoreFromBytes<T>(requestBytes);

            return request;
        }

        public static int ReadRequestLength(byte[] data)
        {
            byte[] sizeBytes = data.Take(4).ToArray();

            int requestLength = checked(ByteConverter.ReadIntFromBytes(sizeBytes));

            if (requestLength < 0)
                throw new ArgumentOutOfRangeException("Request length can't be less than 0");

            return requestLength;
        }
    }
}
