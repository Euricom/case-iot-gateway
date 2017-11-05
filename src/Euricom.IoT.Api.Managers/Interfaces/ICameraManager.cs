using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ICameraManager
    {
        IEnumerable<CameraDto> Get();
        CameraDto Get(string deviceId);
        CameraDto Add(CameraDto camera);
        CameraDto Update(CameraDto camera);
        void Remove(string deviceId);

        Task<bool> TestConnection(string deviceId);
        //void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber);
    }
}
