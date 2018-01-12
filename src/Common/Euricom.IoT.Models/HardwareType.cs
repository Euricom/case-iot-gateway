using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Euricom.IoT.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HardwareType
    {
        [EnumMember(Value = "camera")]
        Camera,
        [EnumMember(Value = "danalock")]
        DanaLock,
        [EnumMember(Value = "lazybone_switch")]
        LazyBoneSwitch,
        [EnumMember(Value = "lazybone_dimmer")]
        LazyBoneDimmer,
        [EnumMember(Value = "wallmount_switch")]
        WallMountSwitch,
    }
}
