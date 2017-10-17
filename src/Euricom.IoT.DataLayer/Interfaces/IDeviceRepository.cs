using System.Collections.Generic;
using Euricom.IoT.Models;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface IDeviceRepository<TDevice> where TDevice : Device
    {
        void Add(TDevice device);
        void Remove(string id);
        TDevice Get(string id);
        List<TDevice> Get();
        void Update(TDevice device);
    }
}