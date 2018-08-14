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

        private DateTime _heal;
        private DateTime _reset;

        public void StartMonitoring()
        {
            _cancellation = new CancellationTokenSource();

            Task.Run(async () => await Monitor(), _cancellation.Token);
        }

        public async Task Monitor()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 1 && _heal.Date != DateTime.Now.Date)
                {
                    _controller.Heal();
                    _heal = DateTime.Today;
                }

                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 4 && _reset.Date != DateTime.Now.Date)
                {
                    await _controller.SoftReset();
                    _reset = DateTime.Today;
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