namespace Euricom.IoT.Interfaces
{
    public interface INodeValue
    {
        ulong Id { get; }
        
        string Type { get; }
        
        ushort Index { get; }
        
        byte Instance { get; }
        
        byte CommandClassId { get; }
      
        string Label { get; }
        string Value { get; set; }
        string Unit { get; }
    }
}