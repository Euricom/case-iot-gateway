using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Messages;
using Euricom.IoT.Security;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class GatewayManager : IGatewayManager
    {
        private MemoryCache _cache;
        private IDanaLockManager _danaLockManager;
        private ILazyBoneManager _lazyBoneManager;

        public GatewayManager()
        {
            _danaLockManager = new DanaLockManager();
            _lazyBoneManager = new LazyBoneManager();
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

            if (String.IsNullOrEmpty(message.CommandToken))
                throw new ArgumentNullException("message.CommandToken");

            //if (String.IsNullOrEmpty(message.DeviceId))
            //    throw new ArgumentNullException("message.DeviceId");

            if (String.IsNullOrEmpty(message.DeviceType))
                throw new ArgumentNullException("message.DeviceType");

            if (String.IsNullOrEmpty(message.Message))
                throw new ArgumentNullException("message.Message");

            // Verify JWT token
            var isValid = JwtSecurity.VerifyJwt(message.CommandToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Command token was not valid.. Signature invalid!");

            return await ExecuteCommand(message);
        }

        private async Task<bool> ExecuteCommand(GatewayMessage message)
        {
            var deviceType = GetDeviceType(message.DeviceType);
            switch (deviceType)
            {
                case HardwareType.Camera:
                    return false;
                case HardwareType.DanaLock:
                    return await HandleDanaLockMessage(message);
                case HardwareType.LazyBoneSwitch:
                    return await HandleLazyBoneMessage(message);
                default:
                    throw new InvalidOperationException("unknown hardware type");
            }
        }

        private async Task<bool> HandleDanaLockMessage(GatewayMessage message)
        {
            try
            {
                if (String.IsNullOrEmpty(message.Message))
                {
                    Logger.Instance.LogWarningWithContext(this.GetType(), "message was empty");
                    return false;
                }

                var danaLockmessage = JsonConvert.DeserializeObject<DanaLockMessage>(message.Message);
                await _danaLockManager.Switch(danaLockmessage.DeviceId, danaLockmessage.Lock == true ? "close" : "open");
                return true;
            }
            catch (Exception ex)
            {
                var deviceId = JsonConvert.DeserializeObject<DanaLockMessage>(message.Message).DeviceId;
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                return false;
            }
        }

        private async Task<bool> HandleLazyBoneMessage(GatewayMessage message)
        {
            try
            {
                if (String.IsNullOrEmpty(message.Message))
                {
                    Logger.Instance.LogWarningWithContext(this.GetType(), "message was empty");
                    return false;
                }

                var lazyBoneMessage = JsonConvert.DeserializeObject<LazyBoneMessage>(message.Message);
                await _lazyBoneManager.Switch(lazyBoneMessage.DeviceId, lazyBoneMessage.On == true ? "on" : "off");
                return true;
            }
            catch (Exception ex)
            {
                var deviceId = JsonConvert.DeserializeObject<LazyBoneMessage>(message.Message).DeviceId;
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
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
                case "lazybone":
                    return HardwareType.LazyBoneSwitch;
                default:
                    throw new NotSupportedException($"deviceType {deviceType} not supported or unknown");
            }
        }
    }
}
