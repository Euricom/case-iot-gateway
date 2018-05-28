using Euricom.IoT.Api.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class DanaLockManager : IDanaLockManager
    {
        private readonly IDeviceRepository<DanaLock> _repository;
        private readonly IZWaveController _controller;
        private readonly IDeviceHubRegistry _deviceRegistry;
        private readonly IGatewayDeviceRegistry _gatewayDeviceRegistry;

        public DanaLockManager(IDeviceRepository<DanaLock> repository, IZWaveController controller, IDeviceHubRegistry deviceRegistry, IGatewayDeviceRegistry gatewayDeviceRegistry)
        {
            _repository = repository;
            _controller = controller;
            _deviceRegistry = deviceRegistry;
            _gatewayDeviceRegistry = gatewayDeviceRegistry;
        }

        public IEnumerable<DanaLockDto> Get()
        {
            var devices = _repository.Get();

            return Mapper.Map<IEnumerable<DanaLockDto>>(devices);
        }

        public DanaLockDto Get(string deviceId)
        {
            var wallmount = _repository.Get(deviceId);

            return Mapper.Map<DanaLockDto>(wallmount);
        }

        public async Task<DanaLockDto> Add(DanaLockDto dto)
        {
            var primaryKey = await _deviceRegistry.AddDeviceAsync(dto.DeviceId, HardwareType.DanaLock);

            var danalock = new DanaLock(dto.DeviceId, primaryKey, dto.NodeId, dto.Name, dto.Enabled, dto.PollingTime);

            _repository.Add(danalock);

            await _gatewayDeviceRegistry.AddDeviceAsync(dto.DeviceId, primaryKey);

            return Mapper.Map<DanaLockDto>(danalock);
        }

        public DanaLockDto Update(DanaLockDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (String.IsNullOrEmpty(dto.DeviceId))
            {
                throw new ArgumentException(nameof(dto.DeviceId));
            }

            var wallmount = _repository.Get(dto.DeviceId);
            wallmount.Update(dto.NodeId, dto.Name, dto.PollingTime, dto.Enabled);

            _repository.Update(wallmount);

            return Mapper.Map<DanaLockDto>(wallmount);
        }

        public async Task Remove(string deviceId)
        {
            await _gatewayDeviceRegistry.RemoveDeviceAsync(deviceId);
            await _deviceRegistry.RemoveDeviceAsync(deviceId);

            _repository.Remove(deviceId);
        }

        public bool TestConnection(string deviceId)
        {
            var device = _repository.Get(deviceId);

            return device.TestConnection(_controller);
        }

        public bool IsLocked(string deviceId)
        {
            var danalock = _repository.Get(deviceId);

            return danalock.IsLocked(_controller);
        }

        public void Switch(string deviceId, string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (state != "open" && state != "close")
            {
                throw new ArgumentException(state);
            }
            
            var danalock = _repository.Get(deviceId);
            
            switch (state)
            {
                case "open":
                    danalock.OpenLock(_controller);
                    break;
                case "close":
                    danalock.CloseLock(_controller);
                    break;
            }
        }
    }
}
