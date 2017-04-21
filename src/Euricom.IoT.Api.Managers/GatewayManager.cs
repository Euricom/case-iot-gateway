using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Messages;
using Euricom.IoT.Security;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class GatewayManager : IGatewayManager
    {
        private IDanaLockManager _danaLockManager;
        private ILazyBoneManager _lazyBoneManager;
        private IWallMountSwitchManager _wallmountSwitchManager;

        public GatewayManager()
        {
            _danaLockManager = new DanaLockManager();
            _lazyBoneManager = new LazyBoneManager();
            _wallmountSwitchManager = new WallMountSwitchManager();
        }

        public async Task Initialize()
        {
            Logger.Instance.LogInformationWithContext(this.GetType(), "Forwarding IoT hub messages from IoTGateway device to hardware");
            var settings = DataLayer.Database.Instance.GetConfigSettings();
            if (settings == null || String.IsNullOrEmpty(settings.GatewayDeviceKey))
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Setting: 'Gateway Device key' was not provided.. Cannot proccess device messages");
                return;
            }

            var deviceGatewayClient = DeviceClient.Create(settings.AzureIotHubUri,
                new DeviceAuthenticationWithRegistrySymmetricKey("IoTGateway", settings.GatewayDeviceKey),
                Microsoft.Azure.Devices.Client.TransportType.Http1);

            await Task.Run(async () =>
             {
                 while (true)
                 {
                     Microsoft.Azure.Devices.Client.Message receivedMessage = await deviceGatewayClient.ReceiveAsync();
                     if (receivedMessage == null) continue;

                     try
                     {

                         var messageString = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                         var gatewayMessage = JsonConvert.DeserializeObject<GatewayMessage>(messageString);

                         Logger.Instance.LogDebugWithContext(this.GetType(), $"Handling message: {messageString}");

                         // Handle message
                         bool messageHandled = await HandleMessage(gatewayMessage);

                         Logger.Instance.LogDebugWithContext(this.GetType(), $"Handling message done: {messageString}");

                         if (messageHandled)
                             await deviceGatewayClient.CompleteAsync(receivedMessage);
                         else
                             await deviceGatewayClient.RejectAsync(receivedMessage);
                     }
                     catch (Exception ex)
                     {
                         Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                         await deviceGatewayClient.RejectAsync(receivedMessage);
                     }
                 }
             });
        }

        public async Task<bool> HandleMessage(GatewayMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.MessageType == null)
                throw new ArgumentNullException("message.MessageType");

            return await HandleCommandMessage((CommandMessage) message);
        }

        private async Task<bool> HandleCommandMessage(CommandMessage message)
        {
            // Verify JWT token
            var isValid = JwtSecurity.VerifyJwt(message.CommandToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Command token was not valid.. Signature invalid!");

            var deviceId = new HardwareManager().GetDeviceId(message.Device);
            switch (message.MessageType)
            {
                case "danalock":
                    return await HandleDanaLockMessage(deviceId, (DanaLockMessage)message);
                case "lazybone_switch":
                    return await HandleLazyBoneMessage(deviceId, (LazyBoneSwitchMessage)message);
                case "lazybone_dimmer":
                    return await HandleLazyBoneMessage(deviceId, (LazyBoneDimmerMessage)message);
                case "wallmount_switch":
                    return await HandleWallMountSwitchMessage(deviceId, (WallmountSwitchMessage)message);
                default:
                    throw new InvalidOperationException("unknown message type");
            }
        }
        private async Task<bool> HandleDanaLockMessage(string deviceId, DanaLockMessage message)
        {
            try
            {
                await _danaLockManager.Switch(deviceId, message.Locked == true ? "close" : "open");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return false;
            }
        }

        private async Task<bool> HandleWallMountSwitchMessage(string deviceId, WallmountSwitchMessage message)
        {
            try
            {
                await _wallmountSwitchManager.Switch(deviceId, message.State == true ? "close" : "open");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return false;
            }
        }

        private async Task<bool> HandleLazyBoneMessage(string deviceId, GatewayMessage message)
        {
            try
            {
                var isLazyBoneSwitch = message.MessageType == "lazybone_switch";
                var isLazyBoneDimmer = message.MessageType == "lazybone_dimmer";

                if (isLazyBoneSwitch)
                {
                    var lazyBoneMsg = (LazyBoneSwitchMessage)message;
                    await _lazyBoneManager.Switch(deviceId, lazyBoneMsg.State == true ? "on" : "off");
                    return true;
                }
                else if (isLazyBoneDimmer)
                {
                    var lazyBoneMsg = (LazyBoneDimmerMessage)message;
                    await _lazyBoneManager.Switch(deviceId, lazyBoneMsg.State == true ? "on" : "off");

                    if (lazyBoneMsg.LightIntensity.HasValue)
                        await _lazyBoneManager.SetLightValue(deviceId, lazyBoneMsg.LightIntensity);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return false;
            }

        }

        private HardwareType GetDeviceType(string deviceType)
        {
            switch (deviceType)
            {
                case "danalock":
                    return HardwareType.DanaLock;
                case "camera":
                    return HardwareType.Camera;
                case "lazybone_switch":
                    return HardwareType.LazyBoneSwitch;
                case "lazybone_dimmer":
                    return HardwareType.LazyBoneDimmer;
                case "wallmount_switch":
                    return HardwareType.WallmountSwitch;
                default:
                    throw new NotSupportedException($"deviceType {deviceType} not supported or unknown");
            }
        }
    }
}
