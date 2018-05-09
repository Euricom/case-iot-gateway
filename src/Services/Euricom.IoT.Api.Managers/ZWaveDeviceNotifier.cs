using System.Threading.Tasks;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class ZWaveDeviceNotifier : IZWaveDeviceNotifier
    {
        private readonly IZWaveDeviceRepository _repository;
        private readonly IGatewayDeviceRegistry _gatewayDeviceRegistry;

        public ZWaveDeviceNotifier(IZWaveDeviceRepository repository, IGatewayDeviceRegistry gatewayDeviceRegistry)
        {
            _repository = repository;
            _gatewayDeviceRegistry = gatewayDeviceRegistry;
        }

        public void Notify(byte nodeId, byte key, byte value)
        {
            var device = _repository.GetZWaveDevice(nodeId);

            if (device != null)
            {
                // Sometimes we get notifications twice
                // We keep the state so we can check 
                if (device.UpdateState(key, value))
                {
                    _repository.UpdateZWaveDevice(device);

                    var state = device.GetState();

                    Task.Run(async () => await _gatewayDeviceRegistry.SendAsync(device.DeviceId, state));
                }
            }
        }
    }
}