using System.IO;
using System.Threading.Tasks;
using Euricom.IoT.Common;

namespace Euricom.IoT.AzureBlobStorage
{
    public interface IAzureBlobStorageManager
    {
        Task<bool> DeleteImageByIdAsync(string containerName, string imageName);
        Task<DropboxFile> GetFileByIdAsync(string containerName, string fileName);
        Task<DropboxFile> PostImage(string containerName, string name, Stream body);
    }
}