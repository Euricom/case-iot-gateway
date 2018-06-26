using System.Collections.Generic;
using Dropbox.Api.Files;
using System.Threading.Tasks;
using System.IO;

namespace Euricom.IoT.Dropbox
{
    public interface IDropbox
    {
        Task<Dictionary<string, byte[]>> DownloadFiles(IList<Metadata> entries);
        Task<IList<Metadata>> PollDropboxNewFiles(string path);
    }
}