using System;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.WallMountSwitch;
using Euricom.IoT.Interfaces;
using System.Collections.Generic;
using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Logging;

namespace Euricom.IoT.Api.Managers
{
    public class WallMountSwitchManager : IWallMountSwitchManager
    {
        private readonly IDeviceRepository<WallMountSwitch> _repository;
        private readonly IZWaveController _controller;
        private readonly IAzureDeviceRegistry _deviceRegistry;

        public WallMountSwitchManager(IDeviceRepository<WallMountSwitch> repository, IZWaveController controller, IAzureDeviceRegistry deviceRegistry)
        {
            _repository = repository;
            _controller = controller;
            _deviceRegistry = deviceRegistry;
        }
        
        public IEnumerable<WallMountSwitchDto> Get()
        {
            var devices = _repository.Get();

            return Mapper.Map<IEnumerable<WallMountSwitchDto>>(devices);
        }

        public WallMountSwitchDto Get(string deviceId)
        {
            var wallmount = _repository.Get(deviceId);
            
            return Mapper.Map<WallMountSwitchDto>(wallmount);
        }

        public async Task<WallMountSwitchDto> Add(WallMountSwitchDto dto)
        {
            var primaryKey = await _deviceRegistry.AddDeviceAsync(dto.DeviceId);
            var wallmount = new WallMountSwitch(dto.DeviceId, primaryKey, dto.NodeId, dto.Name, dto.Enabled, dto.PollingTime);

            _repository.Add(wallmount);

            return Mapper.Map<WallMountSwitchDto>(wallmount);
        }

        public WallMountSwitchDto Update(WallMountSwitchDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (String.IsNullOrEmpty(dto.DeviceId))
            {
                throw new ArgumentException("wallmount.DeviceId");
            }

            var wallmount = _repository.Get(dto.DeviceId);
            wallmount.Update(dto.NodeId, dto.Name, dto.PollingTime, dto.Enabled);
            
            _repository.Update(wallmount);

            return Mapper.Map<WallMountSwitchDto>(wallmount);
        }

        public async Task Remove(string deviceId)
        {
            await _deviceRegistry.RemoveDeviceAsync(deviceId);

            _repository.Remove(deviceId);
        }

        public bool IsOn(string deviceId)
        {
            try
            {
                var device = _repository.Get(deviceId);
                if (!device.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {device.Name} {deviceId} is not enabled");
                }

                return device.IsOn(_controller);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public void Switch(string deviceId, string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new Exception("param state was null or empty");
            }

            if (state != "on" && state != "off")
            {
                throw new Exception($"UNKNOWN parameter: {state}. Please use 'on' or 'off'");
            }

            try
            {
                var device = _repository.Get(deviceId);

                if (!device.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {device.Name} {deviceId} is not enabled");
                }

                switch (state)
                {
                    case "on":
                        device.TurnOn(_controller);
                        break;
                    case "off":
                        device.TurnOff(_controller);
                        break;
                }

                // Log command
                Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed state: {state}");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public bool TestConnection(string deviceId)
        {
            var device = _repository.Get(deviceId);

            return device.TestConnection(_controller);
        }
    }
}
