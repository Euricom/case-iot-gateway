using OpenZWave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.OpenZWave
{
    public sealed class ZWaveRequest
    {
        private DateTimeOffset creationTime;
        public ZWaveRequest()
        {
            creationTime = DateTimeOffset.Now;
        }
        public TimeSpan Age { get { return DateTimeOffset.Now - creationTime; } }
        public NotificationType Type { get; set; }
        public byte HomeID { get; set; }
        public byte NodeID { get; set; }
        public TaskCompletionSource<ZWNotification> TCS { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.MaxValue;
    }
}
