using Euricom.IoT.Interfaces;

namespace Euricom.IoT.ZWave
{
    public class NodeValue : INodeValue
    {
        public ulong Id { get; set; }
        public string Type { get; set; }
        public ushort Index { get; set; }
        public byte Instance { get; set; }
        public byte CommandClassId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}