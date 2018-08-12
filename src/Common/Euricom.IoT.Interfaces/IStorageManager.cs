using System;
using System.IO;
using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IStorageManager
    {
        Task Initialize();
        Task<string> PostImage(string container, string name, Stream body);
    }
}