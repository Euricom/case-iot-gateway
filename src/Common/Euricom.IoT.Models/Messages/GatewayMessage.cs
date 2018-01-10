namespace Euricom.IoT.Models.Messages
{
    public class GatewayMessage
    {
        public string Gateway { get; set; } // Id of gateway
        public string Device { get; set; } //Name of device
        public string MessageType { get; set; } //Type of message: 'lazybone_switch', 'lazybone_dimmer', 'danalock', 'wallmount_switch'
    }
}
