using Euricom.IoT.Common;
using Euricom.IoT.Models;
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
        Task<Camera> GetByDeviceId(string deviceId);
        Task<Camera> GetByDeviceName(string deviceName);
        Task<Camera> Add(Camera camera);
        Task<Camera> Edit(Camera camera);
        Task Remove(string devicename);
        Task<bool> TestConnection(string deviceId);
        void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber);
        string GetDeviceId(string devicename);
    }
}
