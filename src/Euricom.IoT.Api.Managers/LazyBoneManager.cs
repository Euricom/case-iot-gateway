using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Logging;
using System;
using System.Collections.Generic;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.LazyBone;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private readonly IDeviceRepository<LazyBone> _repository;
        private readonly SocketClient _socketClient;

        public LazyBoneManager(IDeviceRepository<LazyBone> repository, SocketClient socketClient)
        {
            _repository = repository;
            _socketClient = socketClient;
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

        public LazyBoneDto Add(LazyBoneDto dto)
        {
            var device = new LazyBone(dto.DeviceId, dto.IsDimmer,dto.Name, dto.Enabled, dto.PollingTime, dto.Name, dto.Port);

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

        public void Remove(string deviceId)
        {
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
