using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class LogLine
    {
        public long Sequence { get; set; }
        public DateTime Time { get; set; }
        public string DeviceName { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
