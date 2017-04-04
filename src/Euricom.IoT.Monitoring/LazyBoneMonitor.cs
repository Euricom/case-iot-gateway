using Euricom.IoT.Api.Managers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Monitoring
{
    public class LazyBoneMonitor
    {
        public LazyBoneMonitor()
        {
        }

        public CancellationTokenSource StartMonitor(Common.LazyBone lazyBone, int pollingTime)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    var notification = await PollLazyBone(lazyBone.DeviceId);

                    PublishNotification(lazyBone, notification);

                    await Task.Delay(pollingTime);
                }
            }, ct);

            return cts;
        }

        private async Task<Common.Notifications.LazyBoneNotification> PollLazyBone(string deviceId)
        {
            var currentState = await new LazyBoneManager().GetCurrentState(deviceId);
            return new Common.Notifications.LazyBoneNotification()
            {
                DeviceKey = deviceId,
                State = currentState,
                Timestamp = Common.Utilities.DateTimeHelpers.Timestamp(),
            };
        }

        private void PublishNotification(Common.LazyBone lazyBone, Common.Notifications.LazyBoneNotification notification)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            new Messaging.MqttMessagePublisher(lazyBone.Name, lazyBone.DeviceId).Publish(json);
        }
    }
}
