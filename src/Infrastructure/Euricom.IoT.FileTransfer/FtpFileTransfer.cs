using Cubisoft.Winrt.Ftp;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Euricom.IoT.FileTransfer
{
    public static class FtpFileTransfer
    {
        private const string Host = "192.168.1.117";

        private static readonly NetworkCredential _credentials;
        private static readonly FtpClient _ftpClient;
     
        static FtpFileTransfer()
        {
            _credentials = new NetworkCredential("admin", "Bonaparte");

            _ftpClient = new FtpClient();

            _ftpClient.Credentials = _credentials;
            _ftpClient.HostName = new HostName(Host);
            _ftpClient.ServiceName = "21";
        }

        public static async Task<byte[]> GetFile(string path)
        {
            try
            {              
                await _ftpClient.ConnectAsync();

                if (!_ftpClient.IsConnected)
                {
                    Debug.WriteLine("Unable to connect to FTP.");
                }

                var file = await _ftpClient.RetrieveFileAsync(path);

                await _ftpClient.SetWorkingDirectoryAsync("/");

                //await _ftpClient.DisconnectAsync();

                return await GetBytesAsync(file);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                await _ftpClient.DisconnectAsync();

                return null;
            }
        }

        private static async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            if (file == null)
            {
                return null;
            }
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
    }
}
