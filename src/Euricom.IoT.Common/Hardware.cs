using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class Hardware
    {
        public List<Camera> Cameras { get; set; }
        public List<LazyBone> Switches { get; set; }
        public List<DanaLock> DanaLocks { get; set; }
    }
}
