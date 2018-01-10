﻿using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IZWaveDeviceNotifier
    {
        Task Notify(byte nodeId, byte commandId, byte value);
    }
}