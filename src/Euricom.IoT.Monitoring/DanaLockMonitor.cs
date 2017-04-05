using Euricom.IoT.Api.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Monitoring
{
    public class DanaLockMonitor
    {
        public DanaLockMonitor()
        {
        }

        public CancellationTokenSource StartMonitor(Common.DanaLock danaLock, int pollingTime)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var notification = await PollDanaLock(danaLock.DeviceId);

                        PublishNotification(danaLock, notification);

                        await Task.Delay(pollingTime);
                    }
                    catch (Exception ex)
                    {
                        //TODO add logging
                        Debug.WriteLine($"Exception occurred while monitoring DanaLock device {danaLock.DeviceId}, exception message: {ex.Message}");
                    }
                }
            }, ct);

            return cts;
        }

        private async Task<Common.Notifications.DanaLockNotification> PollDanaLock(string deviceId)
        {
            var isLocked = await new DanaLockManager().IsLocked(deviceId);
            return new Common.Notifications.DanaLockNotification()
            {
                DeviceKey = deviceId,
                Locked = isLocked,
                Timestamp = Common.Utilities.DateTimeHelpers.Timestamp(),
            };
        }

        private void PublishNotification(Common.DanaLock danaLock, Common.Notifications.DanaLockNotification notification)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            new Messaging.MqttMessagePublisher(danaLock.Name, danaLock.DeviceId).Publish(json);
        }
    }
}
