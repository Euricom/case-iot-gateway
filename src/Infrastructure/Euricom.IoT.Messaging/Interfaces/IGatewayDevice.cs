using System.Collections.Generic;

namespace Euricom.IoT.Messaging.Interfaces
{
    public interface IGatewayDevice
    {
        string DeviceId { get; }

        bool IsRunning();
        void Start();
        void Stop();
        void Send(Dictionary<string, object> properties);
    }
}