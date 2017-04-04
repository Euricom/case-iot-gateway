using Euricom.IoT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ICameraManager
    {
        Task<IEnumerable<Camera>> GetAll();
        Task<Camera> Get(string deviceId);
        Task<Camera> Add(Camera camera);
        Task<Camera> Edit(Camera camera);
        Task<bool> Remove(string deviceId);
        Task<bool> TestConnection(string deviceId);
        void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber);
    }
}
