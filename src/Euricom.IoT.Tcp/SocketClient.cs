using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Tcp
{
    //http://donatas.xyz/streamsocket-tcpip-client.html
    public class SocketClient: ISocketClient
    {
        private static readonly object SyncRoot = new Object();

        public bool TestConnection(string host, short port)
        {
            lock (SyncRoot)
            {
                try
                {
                    using (var tcpClient = new TcpClient { NoDelay = true })
                    {
                        tcpClient.ConnectAsync(host, port).Wait(2000);
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public byte[] Send(string host, short port, string message, bool read)
        {
            lock (SyncRoot)
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.NoDelay = true;
                    tcpClient.SendTimeout = 2000;
                    tcpClient.ReceiveTimeout = 2000;
                    tcpClient.ConnectAsync(host, port).Wait(2000);

                    Task.Delay(100).Wait();

                    using (var stream = tcpClient.GetStream())
                    {
                        ASCIIEncoding asen = new ASCIIEncoding();
                        byte[] ba = asen.GetBytes(message);
                        stream.Write(ba, 0, ba.Length);
                        stream.Flush();

                        if (read)
                        {
                            Task.Delay(2000).Wait();
                            stream.AsInputStream().AsStreamForRead();

                            long position = 0;
                            byte[] buffer = new byte[10];
                            position += stream.Read(buffer, (int)position, (int)(buffer.Length - position));

                            // Buffer is now too big. Shrink it.
                            byte[] ret = new byte[position];
                            Array.Copy(buffer, ret, (int)position);
                            var response = ret;

                            Debug.WriteLine(response.Length);
                            return response;
                        }
                    }
                    return null;
                }
            }
        }
    }

}
