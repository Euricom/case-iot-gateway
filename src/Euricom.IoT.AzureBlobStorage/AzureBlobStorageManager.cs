using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Euricom.IoT.AzureBlobStorage
{
    public class AzureBlobStorageManager : IAzureBlobStorageManager
    {
        private StorageCredentials _credentials;
        CloudStorageAccount _storageAccount;
        CloudBlobClient _blobClient;

        public AzureBlobStorageManager()
        {
            var configSettings = Database.Instance.GetConfigSettings();
            CreateCredentialsConnectToClient(configSettings);
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
                await container.CreateIfNotExistsAsync();

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

        private void CreateCredentialsConnectToClient(Settings settings)
        {
            _credentials = new StorageCredentials(settings.AzureAccountName, settings.AzureStorageAccessKey);
            _storageAccount = new CloudStorageAccount(_credentials, true);
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }
    }
}
