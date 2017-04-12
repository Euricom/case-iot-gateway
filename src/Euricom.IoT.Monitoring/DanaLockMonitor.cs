﻿using Euricom.IoT.Api.Managers;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Notifications;
using System;
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

        public CancellationTokenSource StartMonitor(Euricom.IoT.Models.DanaLock danaLock, int pollingTime)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var settings = DataLayer.Database.Instance.GetConfigSettings();

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var notification = await PollDanaLock(danaLock.DeviceId);

                        PublishNotification(settings, danaLock, notification);

                        await Task.Delay(pollingTime);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                        Logger.Instance.LogErrorWithDeviceContext(danaLock.DeviceId, ex);
                        Debug.WriteLine($"Exception occurred while monitoring DanaLock device {danaLock.DeviceId}, exception message: {ex.Message}");
                    }
                }
            }, ct);

            return cts;
        }

        private async Task<DanaLockNotification> PollDanaLock(string deviceId)
        {
            var isLocked = await new DanaLockManager().IsLocked(deviceId);
            return new DanaLockNotification()
            {
                DeviceKey = deviceId,
                Locked = isLocked,
                Timestamp = Common.Utilities.DateTimeHelpers.Timestamp(),
            };
        }

        private void PublishNotification(Settings settings, Euricom.IoT.Models.DanaLock danaLock, DanaLockNotification notification)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            new Messaging.MqttMessagePublisher(settings, danaLock.Name, danaLock.DeviceId).Publish(json);
        }
    }
}
