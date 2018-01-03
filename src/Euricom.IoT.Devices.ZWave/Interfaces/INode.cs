using System;

namespace Euricom.IoT.Devices.ZWave.Interfaces
{
    public interface INode
    {
        byte Id { get; }
        byte GenericType { get; }
        byte SpecificType { get; }
        string Name { get; }
        string Label { get; }
        string Manufacturer { get; }
        string Product { get; }
    }
}
