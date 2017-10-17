using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.AzureBlobStorage
{
    public class AzureBlobStorageManager : IAzureBlobStorageManager
    {
        private readonly Database _database;
        private StorageCredentials _credentials;
        CloudStorageAccount _storageAccount;
        CloudBlobClient _blobClient;

        public AzureBlobStorageManager(Settings settings, Database database)
        {
            _database = database;
            CreateCredentialsConnectToClient(settings);
        }

        public async Task<DropboxFile> GetFileByIdAsync(string containerName, String fileName)
        {
            containerName = containerName.Replace(@"\", "").Replace(@"/", "");
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockblob = container.GetBlockBlobReference(fileName);
            MemoryStream stream = new MemoryStream();
            await blockblob.DownloadToStreamAsync(stream).ConfigureAwait(false);
            DropboxFile file = new DropboxFile()
            {
                ContentType = blockblob.Properties.ContentType,
                File = stream.ToArray()
            };
            return file;
        }

        public async Task<DropboxFile> PostImage(string containerName, string name, Stream body)
        {
            try
            {
                containerName = containerName.Replace(@"\", "").Replace(@"/", "");

                CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

                try
                {
                    await container.CreateIfNotExistsAsync();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                }

                // Get reference to a blob with name 'imageId' if not excist create new
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
                //blockBlob.Properties.ContentType = MimeTypesMap.GetMimeType(name);
                var upload = blockBlob.UploadFromStreamAsync(body);
                await upload;
                if (upload.Status == TaskStatus.Faulted)
                {
                    throw upload.Exception;
                }
                else
                {
                    var file = Path.GetFileName(blockBlob.Name);
                    return new DropboxFile()
                    {

                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw ex;
            }

        }

        public async Task<bool> DeleteImageByIdAsync(string containerName, string imageName)
        {
            containerName = containerName.Replace(@"\", "").Replace(@"/", "");
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockblob = container.GetBlockBlobReference(imageName);
            return await blockblob.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        public async Task CleanupAsync(string containerName, int maxDays)
        {
            if (maxDays <= 0)
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Azure cleaning not starting, because maxDays was less than or equal to zero");
                return;
            }
            string deviceName = containerName.Replace("/", "");

            var device = _database.GetCameras().SingleOrDefault(x => x.Name == deviceName);
            if (device == null)
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), $"Could not find camera device with name: {deviceName}.. Azure Blob Storage cleanup was not initiated!");
                return;
            }


            containerName = containerName.Replace(@"\", "").Replace(@"/", "");
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            var blobs = await container.ListBlobsSegmentedAsync(null);
            var blobsResults = blobs.Results;
            var filteredBlobs = blobsResults
                       .Cast<CloudBlockBlob>()
                       .Where(x => x.Properties.LastModified.HasValue &&
                                  x.Properties.LastModified.Value.AddDays(maxDays) < DateTime.Now)
                       .ToList();

            if (filteredBlobs.Any())
                Logger.Instance.LogInformationWithDeviceContext(device.DeviceId, $"Deleting {filteredBlobs.Count} files from dropbox");

            foreach (var blob in filteredBlobs)
            {
                await blob.DeleteAsync();
            }
        }

        private void CreateCredentialsConnectToClient(Settings settings)
        {
            _credentials = new StorageCredentials(settings.AzureAccountName, settings.AzureStorageAccessKey);
            _storageAccount = new CloudStorageAccount(_credentials, true);
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }
    }
}
