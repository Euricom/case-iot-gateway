using System.Threading;
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

        public void Notify(byte nodeId, byte key, byte value)
        {
            var device = _repository.GetZWaveDevice(nodeId);

            if (device != null)
            {
                if (device.UpdateState(key, value))
                {
                    _repository.UpdateZWaveDevice(device);

                    var state = device.GetState();

                    MyTaskFactory.StartNew(() => _deviceManager.UpdateStateAsync(device.DeviceId, device.PrimaryKey, state))
                        .Unwrap()
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }
        private static readonly TaskFactory MyTaskFactory =
            new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
    }
}