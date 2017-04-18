using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.LazyBone
{
    //http://donatas.xyz/streamsocket-tcpip-client.html
    public class SocketClient
    {
        private string _hostname;
        private short _port;

        private object _syncRoot = new Object();

        public SocketClient(string hostname, short port)
        {
            _hostname = hostname;
            _port = port;
        }

        public async Task<bool> TestConnection()
        {
            lock (_syncRoot)
            {
                TcpClient tcpClient = new TcpClient();
                try
                {
                    tcpClient = new TcpClient();
                    tcpClient.NoDelay = true;
                    tcpClient.ConnectAsync(_hostname, _port).Wait();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    tcpClient.Dispose();
                }
            }
        }

        public async Task<byte[]> Send(string message, bool readResponse)
        {
            lock (_syncRoot)
            {
                TcpClient tcpClient = new TcpClient();
                Stream stream = null;
                try
                {
                    tcpClient.NoDelay = true;
                    tcpClient.ConnectAsync(_hostname, _port).Wait();
                    Task.Delay(100).Wait();

                    stream = tcpClient.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(message);
                    stream.Write(ba, 0, ba.Length);
                    stream.Flush();

                    if (readResponse)
                    {
                        Task.Delay(2000).Wait();
                        var inputStr = stream.AsInputStream().AsStreamForRead();

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
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    stream.Dispose();
                    tcpClient.Dispose();
                }
            }
        }
    }

}
