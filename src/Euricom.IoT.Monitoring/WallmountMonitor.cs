//using Euricom.IoT.Api.Managers;
//using Euricom.IoT.Common;
//using Euricom.IoT.Logging;
//using Euricom.IoT.Models;
//using Euricom.IoT.Models.Messages;
//using System;
//using System.Diagnostics;
//using System.Threading;
//using System.Threading.Tasks;
//using Euricom.IoT.Api.Managers.Interfaces;
//using Euricom.IoT.DataLayer;

//namespace Euricom.IoT.Monitoring
//{
//    public class WallmountMonitor
//    {
//        private readonly Database _database;
//        private readonly IWallMountSwitchManager _wallMountSwitchManager;

//        public WallmountMonitor(Database database, IWallMountSwitchManager wallMountSwitchManager)
//        {
//            _database = database;
//            _wallMountSwitchManager = wallMountSwitchManager;
//        }

//        public CancellationTokenSource StartMonitor(Euricom.IoT.Models.WallMountSwitch wallmount, int pollingTime)
//        {
//            if (pollingTime < 1000)
//            {
//                throw new ArgumentException("PollingTime must be greater than or equal to 1000 ms");
//            }

//            var cts = new CancellationTokenSource();
//            var ct = cts.Token;

//            var settings = _database.GetConfigSettings();

//            Task.Run(async () =>
//            {
//                while (true)
//                {
//                    try
//                    {
//                        Debug.WriteLine($"MONITOR wallmount {wallmount.DeviceId} is running");

//                        var notification = await PollWallmountSwitch(wallmount.Name, wallmount.DeviceId);

//                        Debug.WriteLine($"MONITOR polling wallmount {wallmount.DeviceId} was done");

//                        PublishNotification(settings, wallmount, notification);

//                        Debug.WriteLine($"MONITOR pushing notification wallmount {wallmount.DeviceId} was done");

//                        await Task.Delay(pollingTime);

//                        Debug.WriteLine($"MONITOR waiting {wallmount.DeviceId} was done");
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.Instance.LogErrorWithContext(this.GetType(), ex);
//                        Logger.Instance.LogErrorWithDeviceContext(wallmount.DeviceId, ex);
//                        Debug.WriteLine($"Exception occurred while monitoring LazyBone device {wallmount.DeviceId}, exception message: {ex.Message}");
//                    }
//                }
//            }, ct);

//            return cts;
//        }

//        private async Task<WallmountSwitchMessage> PollWallmountSwitch(string name, string deviceId)
//        {
//            var currentState = await _wallMountSwitchManager.IsOn(deviceId);
//            return new WallmountSwitchMessage()
//            {
//                Gateway = "IoTGateway",
//                Device = name,
//                State = currentState,
//                CommandToken = null,
//                MessageType = MessageTypes.WallmountSwitch
//            };
//        }

//        private void PublishNotification(Settings settings, Euricom.IoT.Models.WallMountSwitch wallmount, Models.Messages.WallmountSwitchMessage notification)
//        {
//            var json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
//            new Messaging.MqttMessagePublisher(settings, wallmount.Name, wallmount.DeviceId).Publish(json);
//            Debug.WriteLine($"Publishing notification {wallmount.DeviceId} was done");
//        }
//    }
//}
