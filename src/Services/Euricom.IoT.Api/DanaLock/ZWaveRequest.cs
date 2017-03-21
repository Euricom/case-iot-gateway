using OpenZWave;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Euricom.IoT.Api
{
    sealed class ZWaveRequest
    {
        private DateTimeOffset creationTime;

        public ZWaveRequest()
        {
            creationTime = DateTimeOffset.Now;
        }

        public TimeSpan Age { get { return DateTimeOffset.Now - creationTime; } }
        public byte HomeID { get; set; }
        public byte NodeID { get; set; }
        public IAsyncOperation<ZWNotification> TCS { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.MaxValue;
        public NotificationType Type { get; set; }
    }
}
