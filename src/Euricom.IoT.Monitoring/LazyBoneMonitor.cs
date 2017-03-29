using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Monitoring
{
    public class LazyBoneMonitor
    {
        Dictionary<string, CancellationTokenSource> _cancellationTokenSources;

        public LazyBoneMonitor()
        {
            _cancellationTokenSources = new Dictionary<string, CancellationTokenSource>();
        }

        public void StartMonitor(String deviceId, int pollingTime)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            _cancellationTokenSources[deviceId] = cts;

            //var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);
            var lazyBone = new Common.LazyBone()
            {
                DeviceId = deviceId,
                Name = "LazyBone1",
            };

            Task.Run(async () =>
            {
                while (true)
                {
                    var notification = PollLazyBone(deviceId);

                    PublishNotification(lazyBone, notification);

                    await Task.Delay(pollingTime);
                }
            }, ct);
        }

        public void StopMonitor(string deviceId)
        {
            if (_cancellationTokenSources.ContainsKey(deviceId))
            {
                _cancellationTokenSources[deviceId].Cancel();
            }
        }

        public void StopAllMonitors()
        {
            foreach (var cts in _cancellationTokenSources)
            {
                cts.Value.Cancel();
            }
        }

        private Common.Notifications.LazyBoneNotification PollLazyBone(string deviceId)
        {
            //var locked = new Api.Managers.LazyBoneManager().(nodeId);
            return new Common.Notifications.LazyBoneNotification()
            {
                DeviceKey = deviceId,
                State = false,
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
