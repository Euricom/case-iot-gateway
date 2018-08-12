using System.Collections.Generic;
using Euricom.IoT.Models.Messages;

namespace Euricom.IoT.Messaging.Interfaces
{
    public interface IGatewayDevice
    {
        string DeviceId { get; }

        bool IsRunning();
        void Start();
        void Stop();
        void UpdateState(Dictionary<string, object> state);
        void SendMessage(DeviceMessage message);
    }
}