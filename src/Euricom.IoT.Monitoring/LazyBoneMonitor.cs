using Euricom.IoT.Api.Managers;
using Euricom.IoT.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (pollingTime < 1000)
            {
                throw new ArgumentException("PollingTime must be greater than or equal to 1000 ms");
            }

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var settings = DataLayer.Database.Instance.GetConfigSettings();

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        Debug.WriteLine($"MONITOR lazyBone {lazyBone.DeviceId} is running");

                        var notification = await PollLazyBone(lazyBone.DeviceId);

                        Debug.WriteLine($"MONITOR polling lazyBone {lazyBone.DeviceId} was done");

                        PublishNotification(settings, lazyBone, notification);

                        Debug.WriteLine($"MONITOR pushing notification lazyBone {lazyBone.DeviceId} was done");

                        await Task.Delay(pollingTime);

                        Debug.WriteLine($"MONITOR waiting {lazyBone.DeviceId} was done");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                        Logger.Instance.LogErrorWithDeviceContext(lazyBone.DeviceId, ex);
                        Debug.WriteLine($"Exception occurred while monitoring LazyBone device {lazyBone.DeviceId}, exception message: {ex.Message}");
                    }
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

        private void PublishNotification(Common.Settings settings, Common.LazyBone lazyBone, Common.Notifications.LazyBoneNotification notification)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            new Messaging.MqttMessagePublisher(settings, lazyBone.Name, lazyBone.DeviceId).Publish(json);
            Debug.WriteLine($"Publishing notification {lazyBone.DeviceId} was done");
        }
    }
}
