using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models.Notifications
{
    public class LazyBoneLightValueNotification : LazyBoneNotification
    {
        public string Timestamp { get; set; }
        public string DeviceKey { get; set; }
        public int Value { get; set; }
    }
}
