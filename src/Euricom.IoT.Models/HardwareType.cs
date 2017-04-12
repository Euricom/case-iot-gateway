using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HardwareType
    {
        [EnumMember(Value = "camera")]
        Camera,
        [EnumMember(Value = "danalock")]
        DanaLock,
        [EnumMember(Value = "lazybone")]
        LazyBoneSwitch
    }
}
