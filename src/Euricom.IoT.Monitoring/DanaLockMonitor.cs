using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Monitoring
{
    public class DanaLockMonitor
    {
        Dictionary<string, CancellationTokenSource> _cancellationTokenSources;

        public DanaLockMonitor()
        {
            _cancellationTokenSources = new Dictionary<string, CancellationTokenSource>();
        }

        public void StartMonitor(String deviceId, int pollingTime)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            _cancellationTokenSources[deviceId] = cts;

            //var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);
            var config = new Common.DanaLock()
            {
                DeviceId = deviceId,
                Name = "DanaLock1",
                NodeId = 0x4
            };

            Task.Run(async () =>
            {
                while (true)
                {
                    var notification = PollDanaLock(deviceId, config.NodeId);

                    PublishNotification(config, notification);

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

        private Common.Notifications.DanaLockNotification PollDanaLock(string deviceId, byte nodeId)
        {
            var locked = new Api.Managers.DanaLockManager().IsLocked(nodeId);
            return new Common.Notifications.DanaLockNotification()
            {
                DeviceKey = deviceId,
                Locked = locked,
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
