using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.LazyBone;
using Euricom.IoT.Tcp.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private readonly IDeviceRepository<LazyBone> _repository;
        private readonly ISocketClient _socketClient;
        private readonly IAzureDeviceManager _deviceManager;

        public LazyBoneManager(IDeviceRepository<LazyBone> repository, ISocketClient socketClient, IAzureDeviceManager deviceManager)
        {
            _repository = repository;
            _socketClient = socketClient;
            _deviceManager = deviceManager;
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
            var primaryKey = await _deviceManager.AddDeviceAsync(dto.DeviceId);

            var device = new LazyBone(dto.DeviceId, primaryKey, dto.IsDimmer,dto.Name, dto.Enabled, dto.PollingTime, dto.Name, dto.Port);

            _repository.Add(device);

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
            await _deviceManager.RemoveDeviceAsync(deviceId);

            _repository.Remove(deviceId);
        }

        public bool TestConnection(string deviceId)
        {
            try
            {
                var device = _repository.Get(deviceId);

                return device.TestConnection(_socketClient);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }
        
        public void SetState(string deviceId, LazyBoneState state)
        {
            try
            {
                var device = _repository.Get(deviceId);

                device.SetState(_socketClient, state);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public LazyBoneState GetState(string deviceId)
        {
            try
            {
                var lazybone = _repository.Get(deviceId);
                if (!lazybone.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {lazybone.Name} {deviceId} is not enabled");
                }

                return lazybone.GetState(_socketClient);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }
    }
}
