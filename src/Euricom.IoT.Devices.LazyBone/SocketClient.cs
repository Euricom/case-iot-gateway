using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Devices.LazyBone
{
    //http://donatas.xyz/streamsocket-tcpip-client.html
    public class SocketClient
    {
        private readonly string _hostname;
        private readonly short _port;

        private readonly object _syncRoot = new Object();

        public SocketClient(string hostname, short port)
        {
            _hostname = hostname;
            _port = port;
        }

        public bool TestConnection()
        {
            lock (_syncRoot)
            {
                try
                {
                    using (var tcpClient = new TcpClient { NoDelay = true })
                    {
                        tcpClient.ConnectAsync(_hostname, _port).Wait(10000);
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public byte[] Send(string message, bool readResponse)
        {
            lock (_syncRoot)
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.NoDelay = true;
                    tcpClient.SendTimeout = 2000;
                    tcpClient.ReceiveTimeout = 2000;
                    tcpClient.ConnectAsync(_hostname, _port).Wait(2000);

                    Task.Delay(100).Wait();

                    using (var stream = tcpClient.GetStream())
                    {
                        ASCIIEncoding asen = new ASCIIEncoding();
                        byte[] ba = asen.GetBytes(message);
                        stream.Write(ba, 0, ba.Length);
                        stream.Flush();

                        if (readResponse)
                        {
                            Task.Delay(2000).Wait();
                            stream.AsInputStream().AsStreamForRead();

                            long read = 0;
                            byte[] buffer = new byte[10];
                            read += stream.Read(buffer, (int)read, (int)(buffer.Length - read));

                            // Buffer is now too big. Shrink it.
                            byte[] ret = new byte[read];
                            Array.Copy(buffer, ret, (int)read);
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
