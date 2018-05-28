namespace Euricom.IoT.Interfaces
{
    public interface IZWaveDeviceNotificationHandler
    {
        void Notify(byte nodeId, byte commandId, byte value);
    }
}
