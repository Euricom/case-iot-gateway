using Euricom.IoT.Api.Configuration;
using Euricom.IoT.Api.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Euricom.IoT.Api
{
    public sealed class LazyBone
    {
        private readonly string COMMAND_RELAY_TO_1 = ((char)(Int16.Parse("0x65", NumberStyles.AllowHexSpecifier))).ToString();
        private readonly string COMMAND_RELAY_TO_0 = ((char)(Int16.Parse("0x6F", NumberStyles.AllowHexSpecifier))).ToString();
        private readonly string COMMAND_RELAY_STATUS = ((char)(Int16.Parse("0x5B", NumberStyles.AllowHexSpecifier))).ToString();

        private LazyBoneConfig _config;

        public LazyBone(LazyBoneConfig config)
        {
            _config = config;
        }


        public async Task<bool> GetCurrentState()
        {
            var host = _config.Host;
            var port = _config.Port;
            string response;

            using (SocketClient client = new SocketClient())
            {
                await client.connect(_config.Host, _config.Port, COMMAND_RELAY_STATUS);
                response = await client.read(); //Response should be a single byte .. If bit is high , relay is in pos 1

                // TODO debug response and check whether response is bit high
                return true;
            }

        }

        public async void Switch(bool on)
        {

            HostName hostName;
            string port;
            StreamSocket socket;

            try
            {
                hostName = new HostName(_config.Host);
                port = _config.Port;
            }
            catch (ArgumentException)
            {

                return;
            }

            using (socket = new StreamSocket())
            {
                socket.Control.NoDelay = false;

                try
                {
                    await socket.ConnectAsync(hostName, port);


                }
                catch (Exception ex)
                {
                    switch (SocketError.GetStatus(ex.HResult))
                    {
                        case SocketErrorStatus.HostNotFound:
                            throw;
                        default:
                            throw;
                    }
                }

            }

            //var socket = new DatagramSocket();
            //var hostName = new HostName("10.0.1.127:2000");
            //var stream = await socket.GetOutputStreamAsync(hostName, "23");

            //var writer = new DataWriter(stream);
            //writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            //try
            //{
            //    byte[] buffer;
            //    if (on)
            //    {
            //        buffer = Encoding.UTF8.GetBytes("e");
            //    } else
            //    {
            //        buffer = Encoding.UTF8.GetBytes("o");
            //    }
            //    writer.WriteBytes(buffer);
            //    await writer.StoreAsync();
            //}
            //catch (IOException)
            //{
            //    throw;
            //}
        }
    }
}
