using System.IO;
using System.Threading.Tasks;
using Euricom.IoT.Common;

namespace Euricom.IoT.AzureBlobStorage
{
    public interface IAzureBlobStorageManager
    {
        Task<bool> DeleteImageByIdAsync(string imageName);
        Task<DropboxFile> GetFileByIdAsync(string fileName);
        Task<DropboxFile> PostImage(string name, Stream body);
    }
}