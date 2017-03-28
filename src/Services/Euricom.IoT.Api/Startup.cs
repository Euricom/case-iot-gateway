using System;
using Euricom.IoT.Devices.DanaLock;
using System.Collections.Generic;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        private Monitoring.DanaLockMonitor _danaLockMonitor = new Monitoring.DanaLockMonitor();

        public async void Run()
        {
            // Init DanaLock
            await DanaLock.Instance.Initialize();

            // Init Webserver
            await new WebServer().InitializeWebServer();

            // Set up monitoring
            MonitorDanaLocks();
            //MonitorLazyBones();
        }

        private void MonitorDanaLocks()
        {
            //Get all danalocks configs from db (nodeIds)
            //var nodeIds = new List<byte>() { 0x4 };
            //var danalocks = DataLayer.Database.Instance.GetDanaLocks();

            //foreach (var danalock in danalocks)
            //{ 
            //    var pollingTime = 5000; // TODO: get from config
            //    _danaLockMonitor.StartMonitor(danalock.DeviceId, pollingTime);
            //}

            var pollingTime = 5000; // TODO: get from config
            _danaLockMonitor.StartMonitor("3/EELDYvXDZjmggKQla295SuLmUW8Vi5uCnRfhfz5yk=", pollingTime);
        }
    }
}
