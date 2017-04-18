using Euricom.IoT.Api.Managers;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Notifications;
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

        public CancellationTokenSource StartMonitor(Euricom.IoT.Models.LazyBone lazyBone, int pollingTime)
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

                        LazyBoneNotification notification;
                        if (!lazyBone.IsDimmer)
                        {
                            notification = await PollLazyBoneSwitch(lazyBone.DeviceId);
                        }
                        else
                        {
                            notification = await PollLazyBoneDimmer(lazyBone.DeviceId);
                        }

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

        private async Task<LazyBoneSwitchNotification> PollLazyBoneSwitch(string deviceId)
        {
            var currentState = await new LazyBoneManager().GetCurrentStateSwitch(deviceId);
            return new LazyBoneSwitchNotification()
            {
                DeviceKey = deviceId,
                State = currentState,
                Timestamp = Common.Utilities.DateTimeHelpers.Timestamp(),
            };
        }

        private async Task<LazyBoneDimmerNotification> PollLazyBoneDimmer(string deviceId)
        {
            var currentState = await new LazyBoneManager().GetCurrentStateDimmer(deviceId);
            return new LazyBoneDimmerNotification()
            {
                DeviceKey = deviceId,
                State = currentState,
                Timestamp = Common.Utilities.DateTimeHelpers.Timestamp(),
            };
        }

        private void PublishNotification(Settings settings, Euricom.IoT.Models.LazyBone lazyBone, Models.Notifications.LazyBoneNotification notification)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            new Messaging.MqttMessagePublisher(settings, lazyBone.Name, lazyBone.DeviceId).Publish(json);
            Debug.WriteLine($"Publishing notification {lazyBone.DeviceId} was done");
        }
    }
}
