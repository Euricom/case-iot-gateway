using System.Threading.Tasks;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class ZWaveDeviceNotifier : IZWaveDeviceNotifier
    {
        private readonly IZWaveDeviceRepository _repository;
        private readonly IAzureDeviceManager _deviceManager;

        public ZWaveDeviceNotifier(IZWaveDeviceRepository repository, IAzureDeviceManager deviceManager)
        {
            _repository = repository;
            _deviceManager = deviceManager;
        }

        public async Task Notify(byte nodeId, byte key, byte value)
        {
            var device = _repository.GetZWaveDevice(nodeId);

            if (device != null)
            {
                var state = device.GetState(key, value);

                await _deviceManager.UpdateStateAsync(device.DeviceId, state);
            }
        }
    }
}