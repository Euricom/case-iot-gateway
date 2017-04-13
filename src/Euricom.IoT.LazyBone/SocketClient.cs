using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Euricom.IoT.LazyBone
{
    //http://donatas.xyz/streamsocket-tcpip-client.html
    public class SocketClient
    {
        private string _hostname;
        private short _port;

        private TcpClient _tcpclnt;

        public SocketClient(string hostname, short port)
        {
            _tcpclnt = new TcpClient();
            _hostname = hostname;
            _port = port;
        }

        public async Task<byte[]> Send(string message, bool readResponse)
        {
            lock (_tcpclnt)
            {
                Stream stream = null;
                try
                {
                    _tcpclnt = new TcpClient();
                    _tcpclnt.NoDelay = true;
                    //_tcpclnt.ReceiveTimeout = 1000;
                    //_tcpclnt.SendTimeout = 1000;

                    _tcpclnt.ConnectAsync(_hostname, _port).Wait();
                    Task.Delay(100).Wait();

                    stream = _tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(message);
                    stream.Write(ba, 0, ba.Length);
                    stream.Flush();

                    if (readResponse)
                    {
                        Task.Delay(500).Wait();

                        var inputStr = stream.AsInputStream().AsStreamForRead();
                        var response = ReadFully(inputStr, 100);
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
                    _tcpclnt.Dispose();
                }
            }
        }


        /// <summary>
        /// http://www.yoda.arachsys.com/csharp/readbinary.html
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        private static byte[] ReadFully(Stream stream, int initialLength)
        {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            byte[] buffer = new byte[initialLength];
            long read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, (int) read, (int)( buffer.Length - read))) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, (int) read);
            return ret;
        }
    }

}
