using System;
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
        private readonly string _container;

        private CloudBlobContainer _blobContainer;

        public AzureBlobStorageManager(string account, string key, string container)
        {
            _account = account;
            _key = key;
            _container = container;
        }

        public async Task Initialize()
        {
            var credentials = new StorageCredentials(_account, _key);
            var storageAccount = new CloudStorageAccount(credentials, true);
            var client = storageAccount.CreateCloudBlobClient();

            _blobContainer = client.GetContainerReference(_container);
            await _blobContainer.CreateIfNotExistsAsync();
        }

        public async Task<string> PostImage(string name, Stream body)
        {
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(name);

            await blob.UploadFromStreamAsync(body);

            return blob.Uri.ToString();
        }

        public Task CleanupAsync(TimeSpan age)
        {
            throw new NotImplementedException();
        }
    }
}
