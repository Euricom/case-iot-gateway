using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class Hardware
    {
        public Hardware()
        {
            Cameras = new List<Camera>();
            LazyBones = new List<LazyBone>();
            DanaLocks = new List<DanaLock>();
        }

        public List<Camera> Cameras { get; set; }
        public List<LazyBone> LazyBones { get; set; }
        public List<DanaLock> DanaLocks { get; set; }
    }
}
