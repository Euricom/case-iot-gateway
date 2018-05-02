namespace Euricom.IoT.Messaging.Interfaces
{
    public interface IGatewayDeviceFactory
    {
        IGatewayDevice Create(string deviceId, string primaryKey);
    }
}
