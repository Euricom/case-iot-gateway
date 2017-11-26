﻿namespace Euricom.IoT.Tcp.Interfaces
{
    public interface ISocketClient
    {
        bool TestConnection(string host, short port);
        byte[] Send(string host, short port, string message, bool read);
    }
}