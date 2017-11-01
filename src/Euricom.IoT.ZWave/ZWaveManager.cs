using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Euricom.IoT.ZWave.Interfaces;
using OpenZWave;

namespace Euricom.IoT.ZWave
{
    public class ZWaveManager : IZWaveManager
    {
        private readonly string _networkKey;
        private readonly ObservableCollection<Node> _nodeList = new ObservableCollection<Node>();
        private readonly List<ZWaveRequest> _pendingRequests = new List<ZWaveRequest>();
        private readonly ObservableCollection<SerialPortInfo> _serialPorts = new ObservableCollection<SerialPortInfo>();

        private bool _initialized;

        public ZWaveManager(string networkKey)
        {
            _networkKey = networkKey;
            QueryStatus = NodeQueryStatus.Querying;
        }

        public NodeQueryStatus QueryStatus { get; private set; }

        public uint HomeId { get; private set; }
        public string CurrentStatus { get; private set; }
        public static ZWManager ZwManager => ZWManager.Instance;

        public async Task Initialize()
        {
            ZWOptions.Instance.Initialize();
            _initialized = true;

            // Add any app specific options here...

            ZWOptions.Instance.AddOptionString("NetworkKey", _networkKey, false);
            // ordinarily, just write "Detail" level messages to the log
            //m_options.AddOptionInt("SaveLogLevel", (int)ZWLogLevel.Detail);

            // save recent messages with "Debug" level messages to be dumped if an error occurs
            //m_options.AddOptionInt("QueueLogLevel", (int)ZWLogLevel.Debug);

            // only "dump" Debug to the log emessages when an error-level message is logged
            //m_options.AddOptionInt("DumpTriggerLevel", (int)ZWLogLevel.Error);

            // Lock the options
            ZWOptions.Instance.Lock();

            // Create the OpenZWave Manager
            ZwManager.Initialize();
            ZwManager.OnNotification += OnNodeNotification;

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
            if (HomeId > 0 && _nodeList != null && _nodeList.Any(x => x.ID == nodeId))
            {
                return ZwManager.IsNodeAwake(HomeId, nodeId);
            }

            return false;
        }

        public bool GetValue(byte nodeId, byte commandId)
        {
            ZwManager.GetValueAsBool(new ZWValueID(HomeId, nodeId, ZWValueGenre.User, commandId, 1, 0, ZWValueType.Bool, 0), out var currentVal);
            return currentVal;
        }
        public void SetValue(byte nodeId, byte commandId, bool value)
        {
            ZwManager.SetValue(new ZWValueID(HomeId, nodeId, ZWValueGenre.User, commandId, 1, 0, ZWValueType.Bool, 0), value);
        }

        public List<Node> GetNodes()
        {
            var nodes = new List<Node>();
            nodes.AddRange(_nodeList.ToList());

            return nodes;
        }

        public void RemoveNode()
        {
            ZwManager.RemoveNode(HomeId);
        }

        public void AddNode(bool secure)
        {
            ZwManager.AddNode(HomeId, secure);
        }

        public async Task SoftReset()
        {
            if (_initialized && ZWOptions.Instance.AreLocked)
            {
                ZwManager.SoftReset(HomeId);

                await Task.Delay(10000);

                var port = _serialPorts.FirstOrDefault(p => p.IsActive);
                port?.Deactivate(ZwManager);

                _serialPorts.Clear();
                _nodeList.Clear();
                _pendingRequests.Clear();

                ZwManager.OnNotification -= OnNodeNotification;

                ZwManager.Destroy();
                ZWOptions.Instance.Destroy();

                await Task.Delay(10000);
            }

            await Initialize();
        }
        
        #region Notifications
        private void OnNodeNotification(ZWNotification notification)
        {
            var homeID = notification.HomeId;
            var nodeId = notification.NodeId;
            var type = notification.Type;

            if (homeID > 0)
            {
                HomeId = homeID;
            }

            foreach (var item in _pendingRequests.ToArray())
            {
                if (item.HomeID == homeID && item.NodeID == nodeId && item.Type == type)
                {
                    item.TCS.SetResult(notification);
                    _pendingRequests.Remove(item);
                }
                else if (item.Age > item.Timeout)
                {
                    item.TCS.SetException(new TimeoutException());
                    _pendingRequests.Remove(item);
                }
            }

            switch (notification.Type)
            {
                // NodeAdded : Node now exists in the system. Very little useful info
                case NotificationType.NodeAdded:
                    {
                        //if this node was in zwcfg*.xml, this is the first node notification
                        //if not, the NodeNew notification should already have been received
                        if (GetNode(homeID, nodeId) == null)
                            _nodeList.Add(new Node(nodeId, homeID));
                        break;
                    }

                case NotificationType.NodeNew:
                    {
                        //Add the new node to our list(and flag as uninitialized)
                        _nodeList.Add(new Node(nodeId, homeID));
                        break;
                    }

                case NotificationType.NodeRemoved:
                    {
                        foreach (var node in _nodeList)
                        {
                            if (node.ID == nodeId)
                            {
                                _nodeList.Remove(node);
                                node.RaiseNodeRemoved();
                                break;
                            }
                        }
                        break;
                    }

                case NotificationType.NodeProtocolInfo:
                case NotificationType.NodeEvent:
                case NotificationType.NodeNaming:
                case NotificationType.EssentialNodeQueriesComplete:
                case NotificationType.NodeQueriesComplete:
                case NotificationType.ValueRemoved:
                case NotificationType.ValueChanged:
                case NotificationType.Group:
                    {
                        var node = GetNode(homeID, nodeId);
                        node?.HandleNodeEvent(notification);

                        break;
                    }

                case NotificationType.PollingDisabled:
                    {
                        Debug.WriteLine("Polling disabled notification");
                        break;
                    }

                case NotificationType.PollingEnabled:
                    {
                        Debug.WriteLine("Polling enabled notification");
                        break;
                    }

                case NotificationType.DriverReady:
                    {
                        //CurrentStatus = $"Initializing...driver with Home ID 0x{notification.HomeId.ToString("X8")} is ready.";
                        break;
                    }

                case NotificationType.DriverFailed:
                    {
                        Debug.WriteLine("Driver failed for HomeID " + homeID);
                        break;
                    }

                case NotificationType.DriverRemoved:
                    {
                        var nodes = GetNodes(homeID).ToArray();
                        foreach (var node in nodes)
                            _nodeList.Remove(node);
                        break;
                    }

                case NotificationType.AllNodesQueried:
                    {
                        QueryStatus = NodeQueryStatus.AllNodesQueried;
                        Debug.WriteLine("All nodes queried");
                        CurrentStatus = "Ready:  All nodes queried.";
                        ZWManager.Instance.WriteConfig(homeID);
                        break;
                    }

                case NotificationType.AllNodesQueriedSomeDead:
                    {
                        QueryStatus = NodeQueryStatus.AllNodesQueriedSomeDead;
                        Debug.WriteLine("All nodes queried (some dead)");
                        CurrentStatus = "Ready:  All nodes queried but some are dead.";
                        ZWManager.Instance.WriteConfig(homeID);
                        break;
                    }

                case NotificationType.AwakeNodesQueried:
                    {
                        QueryStatus = NodeQueryStatus.AwakeNodesQueried;
                        CurrentStatus = "Ready:  Awake nodes queried (but not some sleeping nodes).";
                        ZWManager.Instance.WriteConfig(homeID);
                        break;
                    }

                default:
                    {
                        if (nodeId > 0)
                        {
                            var node = GetNode(homeID, nodeId);
                            node?.HandleNodeEvent(notification);
                        }
                        else
                        {
                            //var v = GetValue(notification.ValueID);
                            Debug.WriteLine($"******Controller error '{notification.Code}'");
                            Debug.WriteLine($"******Notification '{type}' not Handled @ ID: {nodeId}");
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
            foreach (var node in _nodeList)
            {
                if (node.ID == nodeId && node.HomeID == homeId)
                {
                    return node;
                }
            }
            return null;
        }

        private IEnumerable<Node> GetNodes(uint homeId)
        {
            foreach (var node in _nodeList)
            {
                if (node.HomeID == homeId)
                {
                    yield return node;
                }
            }
        }
        #endregion
    }
}