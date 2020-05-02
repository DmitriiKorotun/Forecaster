using Forecaster.Client.Logging;
using Forecaster.Net;
using Forecaster.Net.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forecaster.Client
{
    public class AsynchronousClient
    {
        // The port number for the remote device.  
        private const int port = 11000;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        private Socket Client { get; set; }

        public void Connect(string hostName)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.   
                IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                Client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), Client);

                connectDone.WaitOne();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Client = (Socket)ar.AsyncState;

                // Complete the connection.  
                Client.EndConnect(ar);

                RamLogger.Log("Socket connected to " + Client.RemoteEndPoint.ToString());
                Console.WriteLine("Socket connected to {0}",
                    Client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendData(byte[] data)
        {
            try
            {
                Send(Client, data);

                sendDone.WaitOne();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Send(Socket client, byte[] data)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = Client.EndSend(ar);
                RamLogger.Log("Sent " + bytesSent + " bytes to server");
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();

                this.Disconnect();
            }
            catch (Exception e)
            {
                this.Disconnect();

                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveData()
        {
            // Receive the response from the remote device.  
            Receive(Client);
            receiveDone.WaitOne();
        }

        public void Disconnect()
        {
            try
            {
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();

                RamLogger.Log("Disconnected from server");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = Client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        RamLogger.Log("Received response: " + state.sb.ToString());
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void Dispose()
        {
            this.Disconnect();
        }
    }
}
