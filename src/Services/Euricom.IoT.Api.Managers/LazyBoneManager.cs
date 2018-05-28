using Euricom.IoT.Api.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.LazyBone;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private readonly IDeviceRepository<LazyBone> _repository;
        private readonly ISocketClient _socketClient;
        private readonly IDeviceHubRegistry _deviceRegistry;
        private readonly IGatewayDeviceRegistry _gatewayDeviceRegistry;

        public LazyBoneManager(IDeviceRepository<LazyBone> repository, ISocketClient socketClient, IDeviceHubRegistry deviceRegistry, IGatewayDeviceRegistry gatewayDeviceRegistry)
        {
            _repository = repository;
            _socketClient = socketClient;
            _deviceRegistry = deviceRegistry;
            _gatewayDeviceRegistry = gatewayDeviceRegistry;
        }

        public IEnumerable<LazyBoneDto> Get()
        {
            var devices = _repository.Get();

            return Mapper.Map<IEnumerable<LazyBoneDto>>(devices);
        }

        public LazyBoneDto Get(string deviceId)
        {
            var device = _repository.Get(deviceId);

            return Mapper.Map<LazyBoneDto>(device);
        }

        public async Task<LazyBoneDto> Add(LazyBoneDto dto)
        {
            var primaryKey = await _deviceRegistry.AddDeviceAsync(dto.DeviceId, dto.IsDimmer ? HardwareType.LazyBoneDimmer : HardwareType.LazyBoneSwitch);

            var device = new LazyBone(dto.DeviceId, primaryKey, dto.IsDimmer, dto.Name, dto.Enabled, dto.PollingTime, dto.Name, dto.Port);

            _repository.Add(device);

            await _gatewayDeviceRegistry.AddDeviceAsync(dto.DeviceId, primaryKey);

            return Mapper.Map<LazyBoneDto>(device);
        }

        public LazyBoneDto Update(LazyBoneDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (String.IsNullOrEmpty(dto.DeviceId))
            {
                throw new ArgumentException(nameof(dto.DeviceId));
            }

            var lazybone = _repository.Get(dto.DeviceId);
            lazybone.Update(dto.Name, dto.Enabled, dto.PollingTime, dto.Host, dto.Port);

            _repository.Update(lazybone);

            return Mapper.Map<LazyBoneDto>(lazybone);
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

            return device.TestConnection(_socketClient);
        }

        public void SetState(string deviceId, LazyBoneState state)
        {
            var device = _repository.Get(deviceId);

            device.SetState(_socketClient, state);
        }

        public LazyBoneState GetState(string deviceId)
        {
            var lazybone = _repository.Get(deviceId);

            return lazybone.GetState(_socketClient);
        }
    }
}
