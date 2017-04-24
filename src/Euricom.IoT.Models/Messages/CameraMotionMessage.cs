using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models.Messages
{
    public class CameraMotionMessage : CommandMessage
    {
        public string FilePath { get; set; }
        public int EventNumber { get; set; }
        public int FrameNumber { get; set; }
    }
}
