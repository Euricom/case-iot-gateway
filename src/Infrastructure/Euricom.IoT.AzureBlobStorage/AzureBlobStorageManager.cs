using System.IO;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Euricom.IoT.AzureBlobStorage
{
    public class AzureBlobStorageManager : IStorageManager
    {
        private readonly string _account;
        private readonly string _key;

        private CloudBlobClient _blobClient;

        public AzureBlobStorageManager(string account, string key)
        {
            _account = account;
            _key = key;
        }

        public Task Initialize()
        {
            var credentials = new StorageCredentials(_account, _key);
            var storageAccount = new CloudStorageAccount(credentials, true);
            _blobClient = storageAccount.CreateCloudBlobClient();

            return Task.CompletedTask;
        }

        public async Task<string> PostImage(string container, string name, Stream body)
        {
            var blobContainer = _blobClient.GetContainerReference(container);
            await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());

            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(name);

            await blob.UploadFromStreamAsync(body);

            return blob.Uri.AbsoluteUri.Replace(blob.Uri.AbsolutePath, blob.Uri.AbsolutePath.Replace(":", "%3A"));
        }
    }
}
