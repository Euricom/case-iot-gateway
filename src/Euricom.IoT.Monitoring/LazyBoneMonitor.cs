using Euricom.IoT.Api.Managers;
using Euricom.IoT.Common;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Messages;
using System;
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

                        LazyBoneMessage message;
                        if (!lazyBone.IsDimmer)
                        {
                            message = await PollLazyBoneSwitch(lazyBone.Name, lazyBone.DeviceId);
                        }
                        else
                        {
                            message = await PollLazyBoneDimmer(lazyBone.Name, lazyBone.DeviceId);
                        }

                        Debug.WriteLine($"MONITOR polling lazyBone {lazyBone.DeviceId} was done");

                        PublishNotification(settings, lazyBone, message);

                        Debug.WriteLine($"MONITOR pushing notification message lazyBone {lazyBone.DeviceId} was done");

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

        private async Task<LazyBoneSwitchMessage> PollLazyBoneSwitch(string name, string deviceId)
        {
            var currentState = await new LazyBoneManager().GetCurrentStateSwitch(deviceId);
            return new LazyBoneSwitchMessage()
            {
                Gateway = "IoTGateway",
                Device = name,
                State = currentState,
                CommandToken = null,
                MessageType = MessageTypes.LazyBoneSwitch
            };
        }

        private async Task<LazyBoneDimmerMessage> PollLazyBoneDimmer(string name, string deviceId)
        {
            var currentState = await new LazyBoneManager().GetCurrentStateDimmer(deviceId);
            return new LazyBoneDimmerMessage()
            {
                Gateway = "IoTGateway",
                Device = name,
                State = currentState.LightOn,
                LightIntensity = currentState.LightValue,
                CommandToken = null,
                MessageType = MessageTypes.LazyBoneDimmer
            };
        }

        private void PublishNotification(Settings settings, Euricom.IoT.Models.LazyBone lazyBone, LazyBoneMessage notification)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            new Messaging.MqttMessagePublisher(settings, lazyBone.Name, lazyBone.DeviceId).Publish(json);
            Debug.WriteLine($"Publishing notification {lazyBone.DeviceId} was done");
        }
    }
}
