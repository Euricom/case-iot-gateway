using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Euricom.IoT.Interfaces;
using OpenZWave;

namespace Euricom.IoT.ZWave
{
    public class ZWaveController : IZWaveController
    {
        private IZWaveDeviceNotifier _notifier;
        
        private readonly ZWManager _zwManager = ZWManager.Instance;
        private readonly ObservableCollection<Node> _nodeList = new ObservableCollection<Node>();
        private readonly ObservableCollection<SerialPortInfo> _serialPorts = new ObservableCollection<SerialPortInfo>();

        private bool _initialized;
        
        private uint _homeId;
        private string _status;
        private string _networkKey;

        public async Task Initialize(IZWaveDeviceNotifier notifier, string networkKey)
        {
            _notifier = notifier;
            _networkKey = networkKey;

            ZWOptions.Instance.Initialize();
            _initialized = true;

            // Add any app specific options here...

            ZWOptions.Instance.AddOptionString("NetworkKey", networkKey, false);
            // ordinarily, just write "Detail" level messages to the log
            //m_options.AddOptionInt("SaveLogLevel", (int)ZWLogLevel.Detail);

            // save recent messages with "Debug" level messages to be dumped if an error occurs
            //m_options.AddOptionInt("QueueLogLevel", (int)ZWLogLevel.Debug);

            // only "dump" Debug to the log emessages when an error-level message is logged
            //m_options.AddOptionInt("DumpTriggerLevel", (int)ZWLogLevel.Error);

            // Lock the options
            ZWOptions.Instance.Lock();

            // Create the OpenZWave Manager
            _zwManager.Initialize();
            _zwManager.NotificationReceived += OnNodeNotification;

#if NETFX_CORE
            var serialPortSelector = Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(serialPortSelector);

            foreach (var item in devices)
            {
                _serialPorts.Add(new SerialPortInfo(item));
            }
#else //.NET
            foreach(var item in System.IO.Ports.SerialPort.GetPortNames())
            {
                SerialPorts.Add(new SerialPortInfo(item));
            }
#endif

            if (!_serialPorts.Any())
            {
                //var _ = new Windows.UI.Popups.MessageDialog("No serial ports found").ShowAsync();
            }
            else if (_serialPorts.Count == 1)
            {
                _serialPorts[0].Activate(ZWManager.Instance); //Assume if there's only one port, that's the ZStick port
            }
            else
            {
                var serial = _serialPorts.SingleOrDefault(x => x.PortID.Contains("VID_0658"));
                serial?.Activate(ZWManager.Instance);
            }

            Debug.WriteLine("OpenZWave initialized");
        }

        public bool TestConnection(byte nodeId)
        {
            if (_homeId > 0 && _nodeList != null && _nodeList.Any(x => x.Id == nodeId))
            {
                // If the zwManager knows it's failed or not awake then we can return false.
                if (_zwManager.IsNodeFailed(_homeId, nodeId) || _zwManager.IsNodeAwake(_homeId, nodeId) == false)
                {
                    return false;
                }

                // Else we need to double check with the controller
                // This is a little tricky because it's async.
                bool receivedResult = false;
                bool result = false;

                var @delegate = new NotificationReceivedEventHandler((sender, args) =>
                {
                    if (args.Notification.HomeId == _homeId && args.Notification.NodeId == nodeId)
                    {
                        receivedResult = true;
                        if (args.Notification.Type == ZWNotificationType.Notification)
                        {
                            result = args.Notification.Code == ZWNotificationCode.Alive || args.Notification.Code == ZWNotificationCode.Awake;
                        }
                    }
                });

                _zwManager.NotificationReceived += @delegate;
                try
                {
                    if (_zwManager.HasNodeFailed(_homeId, nodeId))
                    {
                        // Use a stopwatch as fallback
                        // We don't want to wait more then 5 seconds
                        var sw = new Stopwatch();
                        sw.Start();
                        while (receivedResult == false && sw.ElapsedMilliseconds < 5000)
                        {
                            Thread.Sleep(100);
                        }

                        return result;
                    }
                }
                finally
                {
                    _zwManager.NotificationReceived -= @delegate;
                }
            }

            return false;
        }

        private byte GetValueAsByte(byte nodeId, byte commandId)
        {
            _zwManager.GetValueAsByte(
                new ZWValueId(_homeId, nodeId, ZWValueGenre.User, commandId, 1, 0, ZWValueType.Bool, 0),
                out var currentVal);
            return currentVal;
        }

        public bool GetValue(byte nodeId, byte commandId)
        {
            _zwManager.GetValueAsBool(
                new ZWValueId(_homeId, nodeId, ZWValueGenre.User, commandId, 1, 0, ZWValueType.Bool, 0),
                out var currentVal);
            return currentVal;
        }

        public void SetValue(byte nodeId, byte commandId, bool value)
        {
            _zwManager.SetValue(new ZWValueId(_homeId, nodeId, ZWValueGenre.User, commandId, 1, 0, ZWValueType.Bool, 0),
                value);
        }

        public List<INode> GetNodes()
        {
            var nodes = new List<INode>();
            nodes.AddRange(_nodeList.ToList());

            return nodes;
        }

        public void RemoveNode()
        {
            _zwManager.RemoveNode(_homeId);
        }

        public void AddNode(bool secure)
        {
            _zwManager.AddNode(_homeId, secure);
        }

        public string GetStatus()
        {
            return _status;
        }

        public async Task SoftReset()
        {
            if (_initialized && ZWOptions.Instance.AreLocked)
            {
                _zwManager.SoftReset(_homeId);

                await Task.Delay(10000);

                var port = _serialPorts.FirstOrDefault(p => p.IsActive);
                port?.Deactivate(_zwManager);

                _serialPorts.Clear();
                _nodeList.Clear();

                _zwManager.NotificationReceived -= OnNodeNotification;

                _zwManager.Destroy();
                ZWOptions.Instance.Destroy();

                await Task.Delay(10000);
            }

            await Initialize(_notifier, _networkKey);
        }

        #region Notifications

        private void OnNodeNotification(ZWManager sender, NotificationReceivedEventArgs args)
        {
            var notification = args.Notification;

            var homeId = notification.HomeId;
            var nodeId = notification.NodeId;

            if (homeId > 0)
            {
                _homeId = homeId;
            }

            switch (notification.Type)
            {
                // NodeAdded : Node now exists in the system. Very little useful info
                case ZWNotificationType.NodeAdded:
                    {
                        //if this node was in zwcfg*.xml, this is the first node notification
                        //if not, the NodeNew notification should already have been received
                        if (GetNode(homeId, nodeId) == null)
                        {
                            _nodeList.Add(new Node(nodeId, homeId));
                        }
                        break;
                    }

                case ZWNotificationType.NodeNew:
                    {
                        //Add the new node to our list(and flag as uninitialized)
                        _nodeList.Add(new Node(nodeId, homeId));
                        break;
                    }

                case ZWNotificationType.NodeRemoved:
                    {
                        foreach (var node in _nodeList)
                        {
                            if (node.Id == nodeId)
                            {
                                _nodeList.Remove(node);
                                break;
                            }
                        }
                        break;
                    }

                case ZWNotificationType.DriverReady:
                    {
                        _status = $"Initializing...driver with Home ID 0x{notification.HomeId:X8} is ready.";
                        break;
                    }

                case ZWNotificationType.DriverFailed:
                    {
                        _status = "Driver failed for HomeID " + homeId;
                        break;
                    }

                case ZWNotificationType.DriverRemoved:
                    {
                        var nodes = GetNodes(homeId).ToArray();
                        foreach (var node in nodes)
                        {
                            _nodeList.Remove(node);
                        }
                        break;
                    }

                case ZWNotificationType.AllNodesQueried:
                    {
                        _status = "Ready:  All nodes queried.";

                        ZWManager.Instance.WriteConfig(homeId);
                        break;
                    }

                case ZWNotificationType.AllNodesQueriedSomeDead:
                    {
                        _status = "Ready:  All nodes queried but some are dead.";

                        ZWManager.Instance.WriteConfig(homeId);
                        break;
                    }

                case ZWNotificationType.AwakeNodesQueried:
                    {
                        _status = "Ready:  Awake nodes queried (but not some sleeping nodes).";

                        ZWManager.Instance.WriteConfig(homeId);
                        break;
                    }
                case ZWNotificationType.ValueChanged:
                    {
                        if (nodeId > 0)
                        {
                            var node = GetNode(homeId, nodeId);
                            if (node != null)
                            {
                                _notifier.Notify(nodeId, notification.ValueId.CommandClassId,
                                    Convert.ToByte(GetValue(nodeId, notification.ValueId.CommandClassId)));
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        ///     Gets the node.
        /// </summary>
        /// <param name="homeId">The home identifier.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns></returns>
        private Node GetNode(uint homeId, byte nodeId)
        {
            return _nodeList.FirstOrDefault(node => node.Id == nodeId && node.HomeId == homeId);
        }

        private IEnumerable<Node> GetNodes(uint homeId)
        {
            foreach (var node in _nodeList)
            {
                if (node.HomeId == homeId)
                {
                    yield return node;
                }
            }
        }

        #endregion
    }
}