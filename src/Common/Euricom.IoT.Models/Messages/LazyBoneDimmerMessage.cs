namespace Euricom.IoT.Models.Messages
{
    public class LazyBoneDimmerMessage : LazyBoneMessage
    {
        public bool State { get; set; }
        public short? LightIntensity { get; set; }
    }
}
