using OpenZWave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Euricom.IoT.ZWave
{
    public class ZWaveManager : IZWaveManager
    {
        private static volatile ZWaveManager _instance;
        private static object _syncRoot = new Object();

        private uint _homeId;

        private List<ZWaveRequest> PendingRequests = new List<ZWaveRequest>();

        public ObservableCollection<SerialPortInfo> SerialPorts { get; } = new ObservableCollection<SerialPortInfo>();

        private ObservableCollection<Node> m_nodeList = new ObservableCollection<Node>();


        private ZWaveManager()
        {
        }

        public static ZWaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ZWaveManager();
                    }
                }

                return _instance;
            }
        }

        public uint HomeId
        {
            get
            {
                return _homeId;
            }
        }

        public ZWManager ZWManager
        {
            get
            {
                return ZWManager.Instance;
            }
        }

        private string m_CurrentStatus;

        public string CurrentStatus
        {
            get { return m_CurrentStatus; }
            private set { m_CurrentStatus = value; }
        }

        public enum NodeQueryStatus
        {
            Querying,
            AwakeNodesQueried,
            AllNodesQueried,
            AllNodesQueriedSomeDead
        }

        public NodeQueryStatus QueryStatus { get; private set; } = NodeQueryStatus.Querying;

        public async Task Initialize()
        {
            await Init();

            GetSerialPorts();

            Debug.WriteLine("OpenZWave initialized");
        }

        public bool TestConnection(byte nodeId)
        {
            if (_homeId > 0 && m_nodeList != null && m_nodeList.Any(x=> x.ID == nodeId))
            {
                return true;
            }
            return false;
        }    

        private async Task Init()
        {
            ZWOptions.Instance.Initialize();

            // Add any app specific options here...

            // ordinarily, just write "Detail" level messages to the log
            //m_options.AddOptionInt("SaveLogLevel", (int)ZWLogLevel.Detail);

            // save recent messages with "Debug" level messages to be dumped if an error occurs
            //m_options.AddOptionInt("QueueLogLevel", (int)ZWLogLevel.Debug);

            // only "dump" Debug to the log emessages when an error-level message is logged
            //m_options.AddOptionInt("DumpTriggerLevel", (int)ZWLogLevel.Error);

            // Lock the options
            ZWOptions.Instance.Lock();

            // Create the OpenZWave Manager
            ZWManager.Instance.Initialize();
            ZWManager.Instance.OnNotification += OnNodeNotification;

#if NETFX_CORE
            var serialPortSelector = Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(serialPortSelector);
            foreach (var item in devices)
            {
                SerialPorts.Add(new SerialPortInfo(item));
            }
#else //.NET
                            foreach(var item in System.IO.Ports.SerialPort.GetPortNames())
                            {
                                SerialPorts.Add(new SerialPortInfo(item));
                            }
#endif

        }

        private void OnNodeNotification(ZWNotification notification)
        {
            NotificationHandler(notification);
        }

        private void NotificationHandler(ZWNotification notification)
        {
            var homeID = notification.HomeId;
            var nodeId = notification.NodeId;
            var type = notification.Type;

            if (homeID > 0)
            {
                _homeId = homeID;
            }

            Action<ZWValueID> debugWriteValueID = (v) =>
            {
                //Debug.WriteLine("  Node : " + nodeId.ToString());
                // Debug.WriteLine("  CC   : " + v.CommandClassId.ToString());
                // Debug.WriteLine("  Type : " + type.ToString());
                // Debug.WriteLine("  Index: " + v.Index.ToString());
                // Debug.WriteLine("  Inst : " + v.Instance.ToString());
                // Debug.WriteLine("  Value: " + GetValue(v).ToString());
                // Debug.WriteLine("  Label: " + m_manager.GetValueLabel(v));
                // Debug.WriteLine("  Help : " + m_manager.GetValueHelp(v));
                // Debug.WriteLine("  Units: " + m_manager.GetValueUnits(v));
            };

            foreach (var item in PendingRequests.ToArray())
            {
                if (item.HomeID == homeID && (item.NodeID == nodeId || item.NodeID < 0) && item.Type == type)
                {
                    item.TCS.SetResult(notification);
                    PendingRequests.Remove(item);
                }
                else if (item.Age > item.Timeout)
                {
                    item.TCS.SetException(new TimeoutException());
                    PendingRequests.Remove(item);
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
                        {
                            m_nodeList.Add(new Node(nodeId, homeID));
                        }
                        break;
                    }

                case NotificationType.NodeNew:
                    {
                        //Add the new node to our list(and flag as uninitialized)
                        m_nodeList.Add(new Node(nodeId, homeID));
                        break;
                    }

                case NotificationType.NodeRemoved:
                    {
                        foreach (Node node in m_nodeList)
                        {
                            if (node.ID == nodeId)
                            {
                                m_nodeList.Remove(node);
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
                        Node node = GetNode(homeID, nodeId);
                        if (node != null)
                        {
                            node.HandleNodeEvent(notification);
                        }
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
                        Debug.WriteLine("Driver failed for HomeID " + homeID.ToString());
                        break;
                    }

                case NotificationType.DriverRemoved:
                    {
                        var nodes = GetNodes(homeID).ToArray();
                        foreach (var node in nodes)
                            m_nodeList.Remove(node);
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
                            Node node = GetNode(homeID, nodeId);
                            if (node != null)
                            {
                                node.HandleNodeEvent(notification);
                            }
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
        /// Gets the node.
        /// </summary>
        /// <param name="homeId">The home identifier.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns></returns>
        private Node GetNode(UInt32 homeId, Byte nodeId)
        {
            foreach (Node node in m_nodeList)
            {
                if ((node.ID == nodeId) && (node.HomeID == homeId))
                {
                    return node;
                }
            }

            return null;
        }

        private IEnumerable<Node> GetNodes(UInt32 homeId)
        {
            foreach (Node node in m_nodeList)
            {
                if (node.HomeID == homeId)
                {
                    yield return node;
                }
            }
        }

        private void GetSerialPorts()
        {
            if (!SerialPorts.Any())
            {
                //var _ = new Windows.UI.Popups.MessageDialog("No serial ports found").ShowAsync();
            }
            else if (SerialPorts.Count == 1)
            {
                SerialPorts[0].IsActive = true; //Assume if there's only one port, that's the ZStick port
            }
            else
            {
                var serial = SerialPorts.SingleOrDefault(x => x.PortID.Contains("VID_0658"));
                if (serial != null)
                {
                    serial.IsActive = true;
                }
            }
        }

    }
}
