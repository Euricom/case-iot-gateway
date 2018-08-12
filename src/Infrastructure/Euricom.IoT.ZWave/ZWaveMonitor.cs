using System;
using System.Threading;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.ZWave
{
    public class ZWaveMonitor : IMonitor
    {
        private readonly IZWaveController _controller;

        public ZWaveMonitor(IZWaveController controller)
        {
            _controller = controller;
        }

        private CancellationTokenSource _cancellation;

        public void StartMonitoring()
        {
            _cancellation = new CancellationTokenSource();

            Task.Run(async () => await Monitor(), _cancellation.Token);
        }

        public async Task Monitor()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 1)
                {
                    _controller.Heal();
                }

                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 4)
                {
                    await _controller.SoftReset();
                }

                await Task.Delay(60000);
            }
        }

        public void StopMonitoring()
        {
            _cancellation.Cancel();
        }
    }
}