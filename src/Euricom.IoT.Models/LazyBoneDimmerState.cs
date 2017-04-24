using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class LazyBoneDimmerState
    {
        public bool LightOn { get; set; }
        public short? LightValue { get; set; }
    }
}
