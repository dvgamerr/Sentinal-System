using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Travox.Sentinel.Engine
{
    public class HandleClientRequest
    {
        TcpClient _clientSocket;
        NetworkStream _networkStream = null;
        public HandleClientRequest(TcpClient clientConnected)
        {
            Console.WriteLine("Client Conected");
            this._clientSocket = clientConnected;
        }
        public void StartClient()
        {
            _networkStream = _clientSocket.GetStream();
            WaitForRequest();
        }

        public void WaitForRequest()
        {
            if (_clientSocket.Connected)
            {
                Byte[] buffer = new Byte[_clientSocket.ReceiveBufferSize];
                _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream = _clientSocket.GetStream();
            try
            {
                Int32 read = networkStream.EndRead(result);
                if (read == 0)
                {
                    _networkStream.Close();
                    _clientSocket.Close();
                    return;
                }

                String data = Encoding.Default.GetString(result.AsyncState as Byte[], 0, read);
                Byte[] sendBytes = Encoding.ASCII.GetBytes("Processed " + data);
                networkStream.Write(sendBytes, 0, sendBytes.Length);
                networkStream.Flush();
            }
            catch (Exception ex)
            {
            }

            this.WaitForRequest();
        }
    }
}
