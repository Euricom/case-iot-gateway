using System;
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
        private string _port;

        //StreamSocket _socket;

        public SocketClient(string hostname, string port)
        {
            _hostname = hostname;
            _port = port;
        }

        /// <summary>
        /// Connect to server on port and send message
        /// </summary>
        /// <param name="host">Host name/IP address</param>
        /// <param name="port">Port number</param>
        /// <param name="message">Message to server</param>
        /// <returns>Response from server</returns>
        public async Task Connect()
        {
            HostName hostName;

            var socket = new StreamSocket();

            hostName = new HostName(_hostname);

            // Set NoDelay to false so that the Nagle algorithm is not disabled
            socket.Control.NoDelay = false;

            try
            {
                // Connect to the server
                await socket.ConnectAsync(hostName, _port);
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
            finally
            {
                socket.Dispose();
            }
        }

        /// <summary>
        /// SEND DATA
        /// </summary>
        /// <param name="message">Message to server</param>
        /// <returns>void</returns>
        public async Task<string> Send(string message)
        {
            DataWriter writer;
            DataReader reader;

            var socket = new StreamSocket();
            socket.Control.NoDelay = true;
            await socket.ConnectAsync(new HostName(_hostname), _port);

            //Delay
            await Task.Delay(1500);

            // Create the data writer object backed by the in-memory stream. 
            writer = new DataWriter(socket.OutputStream);
            reader = new DataReader(socket.InputStream);

            // Set the DataReader to only wait for available data (so that we don't have to know the data size)
            reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
            // The encoding and byte order need to match the settings of the writer we previously used.
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

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
                // In order to prolong the lifetime of the stream, detach it from the DataWriter
                writer.DetachStream();

                var strBuilder = new StringBuilder();

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

                var str =  strBuilder.ToString();

                byte[] buffer = Encoding.ASCII.GetBytes(str.ToString());
                return str;
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
            finally
            {
                writer.Dispose();
                socket.Dispose();
            }
        }
    }
}
