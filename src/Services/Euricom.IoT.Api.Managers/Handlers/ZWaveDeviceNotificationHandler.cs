using System.Threading.Tasks;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Api.Managers.Handlers
{
    public class ZWaveDeviceNotificationHandler : IZWaveDeviceNotificationHandler
    {
        private readonly IZWaveDeviceRepository _repository;
        private readonly IGatewayDeviceRegistry _gatewayDeviceRegistry;

        public ZWaveDeviceNotificationHandler(IZWaveDeviceRepository repository, IGatewayDeviceRegistry gatewayDeviceRegistry)
        {
            _repository = repository;
            _gatewayDeviceRegistry = gatewayDeviceRegistry;
        }

        public void Notify(byte nodeId, byte key, byte value)
        {
            try
            {
                var device = _repository.GetZWaveDevice(nodeId);

                // Sometimes we get notifications twice
                // We keep the state so we can check 
                if (device.UpdateState(key, value))
                {
                    _repository.UpdateZWaveDevice(device);

                    var state = device.GetState();

                    Task.Run(async () => await _gatewayDeviceRegistry.UpdateStateAsync(device.DeviceId, state));
                }
            }
            catch (NotFoundException)
            { }
        }
    }
}