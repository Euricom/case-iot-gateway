using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ICameraManager
    {
        IEnumerable<CameraDto> Get();
        CameraDto Get(string deviceId);
        Task<CameraDto> Add(CameraDto camera);
        CameraDto Update(CameraDto camera);
        Task Remove(string deviceId);

        Task<bool> TestConnection(string deviceId);
        Task Notify(string deviceId, string fileName, DateTime timestamp);
        Task<string> GetPicture(string deviceId, Guid? correlationId);
    }
}
