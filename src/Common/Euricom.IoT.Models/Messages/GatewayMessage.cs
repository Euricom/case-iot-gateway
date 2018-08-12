namespace Euricom.IoT.Models.Messages
{
    public class GatewayMessage
    {
        public string Gateway { get; set; } // Id of gateway
        public string Device { get; set; } //Name of device
        //Type of message: 'lazybone_switch', 'lazybone_dimmer', 'danalock', 'wallmount_switch', 'camera'
        public string MessageType { get; set; }
    }
}
