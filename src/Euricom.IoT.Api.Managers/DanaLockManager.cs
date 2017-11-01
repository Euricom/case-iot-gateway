using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Logging;
using System;
using System.Collections.Generic;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.DanaLock;
using IZWaveManager = Euricom.IoT.ZWave.Interfaces.IZWaveManager;

namespace Euricom.IoT.Api.Managers
{
    public class DanaLockManager : IDanaLockManager
    {
        private readonly IDeviceRepository<DanaLock> _repository;
        private readonly IZWaveManager _manager;

        public DanaLockManager(IDeviceRepository<DanaLock> repository, IZWaveManager manager)
        {
            _repository = repository;
            _manager = manager;
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

        public DanaLockDto Add(DanaLockDto dto)
        {
            var danalock = new DanaLock(dto.NodeId, dto.Name, dto.Enabled, dto.PollingTime);

            _repository.Add(danalock);

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
                throw new ArgumentException("wallmount.DeviceId");
            }

            var wallmount = _repository.Get(dto.DeviceId);
            wallmount.Update(dto.NodeId, dto.Name, dto.PollingTime, dto.Enabled);

            _repository.Update(wallmount);

            return Mapper.Map<DanaLockDto>(wallmount);
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

                return device.TestConnection(_manager);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public bool IsLocked(string deviceId)
        {
            try
            {
                var danalock = _repository.Get(deviceId);
                if (!danalock.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {danalock.Name} {deviceId} is not enabled");
                }

                return danalock.IsLocked(_manager);
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

            if (state != "open" && state != "close")
            {
                throw new Exception($"UNKNOWN parameter: { state}. Please use 'open' or 'close'");
            }

            try
            {
                var danalock = _repository.Get(deviceId);
                if (!danalock.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {danalock.Name} {deviceId} is not enabled");
                }

                switch (state)
                {
                    case "open":
                        danalock.OpenLock(_manager);
                        break;
                    case "close":
                        danalock.CloseLock(_manager);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for DanaLock node: {deviceId}, state: {state}");
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
    }
}
