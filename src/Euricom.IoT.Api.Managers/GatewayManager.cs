using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Messages;
using Euricom.IoT.Security;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
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

        public async Task<bool> HandleMessage(GatewayMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (String.IsNullOrEmpty(message.CommandToken))
                throw new ArgumentNullException("message.CommandToken");

            if (String.IsNullOrEmpty(message.DeviceId))
                throw new ArgumentNullException("message.DeviceId");

            if (String.IsNullOrEmpty(message.Message))
                throw new ArgumentNullException("message.Message");

            // Verify JWT token
            var isValid = JwtSecurity.VerifyAccessTokenJwt(message.CommandToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Command token was not valid.. Signature invalid!");

            return await ExecuteCommand(message);
        }

        private async Task<bool> ExecuteCommand(GatewayMessage message)
        {
            // TODO add caching ?
            var device = Database.Instance.FindDevice(message.DeviceId);
            switch (device.Type)
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
                var danaLockmessage = JsonConvert.DeserializeObject<DanaLockMessage>(message.Message);
                await _danaLockManager.Switch(danaLockmessage.DeviceId, danaLockmessage.Lock == true ? "close" : "open");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(message.DeviceId, ex);
                return false;
            }
        }

        private async Task<bool> HandleLazyBoneMessage(GatewayMessage message)
        {
            try
            {
                var lazyBoneMessage = JsonConvert.DeserializeObject<LazyBoneMessage>(message.Message);
                await _lazyBoneManager.Switch(lazyBoneMessage.DeviceId, lazyBoneMessage.On == true ? "on" : "off");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(message.DeviceId, ex);
                return false;
            }

        }
    }
}
