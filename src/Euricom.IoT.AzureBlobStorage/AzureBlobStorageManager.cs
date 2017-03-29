using Euricom.IoT.Common;
using Euricom.IoT.Common.Secrets;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.AzureBlobStorage
{
    public class AzureBlobStorageManager : IAzureBlobStorageManager
    {
        private string _connectionString = "https://mjrstorage.blob.core.windows.net/";
        private string _accountName = Secrets.AZURE_ACCOUNT_NAME;
        private string _containerName = Secrets.AZURE_CONTAINER_NAME;
        private string _azureStorageAccessKey = Secrets.AZURE_STORAGE_ACCESS_KEY;

        private StorageCredentials credentials;
        CloudStorageAccount storageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer container;

        public AzureBlobStorageManager()
        {
            if (true)
            {
                //get the storage account.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);

                //blob client now
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                //the container for this is companystyles
                CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

                //Create a new container, if it does not exist
                container.CreateIfNotExistsAsync();
            }
        }

        public async Task<DropboxFile> GetFileByIdAsync(String fileName)
        {
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

        public async Task<DropboxFile> PostImage(String name, Stream body)
        {
            try
            {
                // Get reference to a blob with name 'imageId' if not excist create new
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
                //blockBlob.Properties.ContentType = MimeTypesMap.GetMimeType(name);
                var test = blockBlob.UploadFromStreamAsync(body);
                await test;
                if (test.Status == TaskStatus.Faulted)
                {
                    throw test.Exception;
                }
                else
                {
                    var file =  Path.GetFileName(blockBlob.Name);
                    return new DropboxFile()
                    {

                    };
                }
            }
            catch (Exception error)
            {
                throw error;
            }

        }

        public async Task<bool> DeleteImageByIdAsync(String imageName)
        {
            // Get reference to a blob with name 'imageId'   
            CloudBlockBlob blockblob = container.GetBlockBlobReference(imageName);
            return await blockblob.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        private void CreateCredentialsConnectToClientAndGetContainer()
        {
            credentials = new StorageCredentials(_containerName, _accountName, _azureStorageAccessKey);
            storageAccount = new CloudStorageAccount(credentials, true);
            // Create a blob client.
            blobClient = storageAccount.CreateCloudBlobClient();
            // Get a reference to a container named “mjr-images.”
            container = blobClient.GetContainerReference(_containerName);
        }
    }
}
