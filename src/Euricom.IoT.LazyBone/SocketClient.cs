using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Euricom.IoT.LazyBone
{
    //http://donatas.xyz/streamsocket-tcpip-client.html
    public class SocketClient
    {
        private Timer _timer;
        private StreamSocket _socket;
        private async Task<StreamSocket> GetSocket()
        {
            if (_socket != null)
            {
                _timer?.Change(TimeSpan.FromSeconds(30), new TimeSpan(-1));
                return _socket;
            }
            _socket = await CreateNewSocket();
            return _socket;
        }

        private async Task<StreamSocket> CreateNewSocket()
        {
            var host = "10.0.1.127";
            var port = "2000";
            var hostName = new HostName(host);

            _timer = new Timer(CloseSocket, null, TimeSpan.FromSeconds(30), new TimeSpan(-1));
            var socket = new StreamSocket();

            // Set NoDelay to false so that the Nagle algorithm is not disabled
            socket.Control.NoDelay = false;

            // Connect to the server
            await socket.ConnectAsync(hostName, port);

            await Task.Delay(1000);
            return socket;
        }

        private void CloseSocket(object state)
        {
            StreamSocket socket = null;
            Interlocked.Exchange(ref socket, _socket);
            socket.Dispose();
        }

        private static SocketClient _instance;

        public static SocketClient Instance => _instance ?? (_instance = new SocketClient());

        private SocketClient()
        {
        }

        /// <summary>
        /// SEND DATA
        /// </summary>
        /// <param name="message">Message to server</param>
        /// <returns>void</returns>
        public async Task Send(string message)
        {
            DataWriter writer;

            // Create the data writer object backed by the in-memory stream. 
            using (writer = new DataWriter((await GetSocket()).OutputStream))
            {
                // Set the Unicode character encoding for the output stream
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                // Specify the byte order of a stream.
                writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                // Gets the size of UTF-8 string.
                writer.MeasureString(message);
                // Write a string value to the output stream.
                writer.WriteString(message);

                // Send the contents of the writer to the backing stream.
                try
                {
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                }
                catch (Exception exception)
                {
                    switch (SocketError.GetStatus(exception.HResult))
                    {
                        case SocketErrorStatus.HostNotFound:
                            // Handle HostNotFound Error
                            throw;
                        default:
                            // If this is an unknown status it means that the error is fatal and retry will likely fail.
                            throw;
                    }
                }

                // In order to prolong the lifetime of the stream, detach it from the DataWriter
                writer.DetachStream();
            }
        }

        /// <summary>
        /// READ RESPONSE
        /// </summary>
        /// <returns>Response from server</returns>
        public async Task<String> Read()
        {
            DataReader reader;
            StringBuilder strBuilder;

            using (reader = new DataReader((await GetSocket()).InputStream))
            {
                strBuilder = new StringBuilder();

                // Set the DataReader to only wait for available data (so that we don't have to know the data size)
                reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
                // The encoding and byte order need to match the settings of the writer we previously used.
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                // Send the contents of the writer to the backing stream. 
                // Get the size of the buffer that has not been read.
                await reader.LoadAsync(256);

                // Keep reading until we consume the complete stream.
                while (reader.UnconsumedBufferLength > 0)
                {
                    strBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));
                    await reader.LoadAsync(256);
                }

                reader.DetachStream();
                return strBuilder.ToString();
            }
        }
    }
}
