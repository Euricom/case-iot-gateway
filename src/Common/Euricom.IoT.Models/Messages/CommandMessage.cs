﻿namespace Euricom.IoT.Models.Messages
{
    public class CommandMessage : GatewayMessage
    {
        public string CommandToken { get; set; }
        public string User { get; set; }
    }
}
