namespace Euricom.IoT.Models.Messages
{
    public abstract class DeviceMessage
    {
        protected DeviceMessage(string messageType)
        {
            MessageType = messageType;
        }

        public string MessageType { get; private set; }
    }
}