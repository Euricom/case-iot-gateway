﻿using OpenZWave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.OpenZWave
{
    public class Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node(byte nodeId, uint homeId)
        {
            ID = nodeId;
            HomeID = homeId;
        }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Byte ID { get; }

        /// <summary>
        /// All the initialization queries on a node have been completed.
        /// </summary>
        public bool NodeQueriesComplete { get; private set; }

        /// <summary>
        /// The queries on a node that are essential to its operation have been completed. 
        /// The node can now handle incoming messages.
        /// </summary>
        public bool EssentialNodeQueriesComplete { get; private set; }

        /// <summary>
        /// Check if the Controller Believes a Node has Failed. This is different from the
        /// IsNodeFailed call in that we test the Controllers Failed Node List, whereas the
        /// IsNodeFailed is testing our list of Failed Nodes, which might be different. The
        /// Results will be communicated via Notifications. Specifically, you should monitor
        /// the ControllerCommand notifications
        /// </summary>
        public bool HasNodeFailed => ZWManager.Instance.HasNodeFailed(HomeID, ID);

        /// <summary>
        /// Gets or sets the home identifier.
        /// </summary>
        /// <value>
        /// The home identifier.
        /// </value>
        public UInt32 HomeID { get; }

        /// <summary>
        /// Gets or sets the name of the device. If the device supports it, this name is stored on the device.
        /// If not, it's only stored in the local device database, and could be lost if the database is lost or the app reset.
        /// </summary>
        public String Name
        {
            get { return ZWManager.Instance.GetNodeName(HomeID, ID); }
            set
            {
                ZWManager.Instance.SetNodeName(HomeID, ID, value);
            }
        }

        /// <summary>
        /// Gets an icon uri matching the <see cref="GenericType"/>.
        /// </summary>
        public Uri DeviceIcon => new Uri($"ms-appx:///DeviceIcons/{GenericType}.png");

        /// <summary>
        /// Get a node's "generic" type.
        /// </summary>
        public byte GenericType => ZWManager.Instance.GetNodeGeneric(HomeID, ID);

        /// <summary>
        /// Get a node's "specific" type.
        /// </summary>
        public byte SpecificType => ZWManager.Instance.GetNodeSpecific(HomeID, ID);

        /// <summary>
        /// Gets or sets the location of the device. If the device supports it, this location is stored on the device.
        /// If not, it's only stored in the local device database, and could be lost if the database is lost or the app reset.
        /// </summary>
        public String Location
        {
            get { return ZWManager.Instance.GetNodeLocation(HomeID, ID); }
            set
            {
                ZWManager.Instance.SetNodeLocation(HomeID, ID, value);
            }
        }

        /// <summary>
        /// Gets a human-readable label describing the node.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public String Label => ZWManager.Instance.GetNodeType(HomeID, ID);

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public String Manufacturer => ZWManager.Instance.GetNodeManufacturerName(HomeID, ID);

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public String Product => ZWManager.Instance.GetNodeProductName(HomeID, ID);

        /// <summary>
        /// Returns an appropriate string combination of <see cref="Manufacturer"/> and <see cref="Product"/>
        /// </summary>
        public string ManufacturerAndProduct
        {
            get
            {
                if (!string.IsNullOrEmpty(Manufacturer) && !string.IsNullOrEmpty(Product))
                {
                    return $"{Manufacturer} - {Product}";
                }
                else if (!string.IsNullOrEmpty(Manufacturer))
                    return Manufacturer;
                else return Product;
            }
        }

        /// <summary>Gets the basic set of device values.</summary>
        public IList<ZWValueID> BasicValues { get; } = new ObservableCollection<ZWValueID>();

        /// <summary>Gets the basic set of user values.</summary>
        public IList<ZWValueID> UserValues { get; } = new ObservableCollection<ZWValueID>();

        /// <summary>Gets the basic set of system values.</summary>
        public IList<ZWValueID> SystemValues { get; } = new ObservableCollection<ZWValueID>();

        /// <summary>Gets the basic set of configuration values.</summary>
        public IList<ZWValueID> ConfigValues { get; } = new ObservableCollection<ZWValueID>();

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="valueID">The value identifier.</param>
        private void AddValue(ZWValueID valueID)
        {
            IList<ZWValueID> list = GetValues(valueID.Genre);

            var id = list.Where(v => v.CommandClassId == valueID.CommandClassId && v.Id == valueID.Id).FirstOrDefault();
            if (id != null)
            {
                list[list.IndexOf(id)] = valueID; //Update
            }
            else
            {
                list.Add(valueID); //New
            }
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="valueID">The value identifier.</param>
        private void RemoveValue(ZWValueID valueID)
        {
            var values = GetValues(valueID.Genre);
            values.Remove(valueID);
        }

        private IList<ZWValueID> GetValues(ZWValueGenre genre)
        {
            if (genre == ZWValueGenre.Basic)
                return BasicValues;
            else if (genre == ZWValueGenre.Config)
                return ConfigValues;
            else if (genre == ZWValueGenre.System)
                return SystemValues;
            else if (genre == ZWValueGenre.User)
                return UserValues;
            return null;
        }

        /// <summary>
        /// Called by the <see cref="NodeWatcher"/> when events specific to this instance occur
        /// </summary>
        /// <param name="notification"></param>
        internal void HandleNodeEvent(ZWNotification notification)
        {
            switch (notification.Type)
            {
                // NodeProtocolInfo: We now know what type of node it is
                case NotificationType.NodeProtocolInfo:
                    {
                        // Basic node information has been received, such as whether the node is a listening
                        // device, a routing device and its baud rate and basic, generic and specific types.
                        // It is after this notification that you can call get the Label containing the device description.
                        break;
                    }
                case NotificationType.NodeNaming:
                    {
                        // One of the node names has changed (name, manufacturer, product)
                        break;
                    }
                case NotificationType.EssentialNodeQueriesComplete:
                    {
                        // The queries on a node that are essential to its operation have been completed. 
                        // The node can now handle incoming messages.
                        EssentialNodeQueriesComplete = true;
                        break;
                    }
                case NotificationType.NodeQueriesComplete:
                    {
                        // All the initialization queries on a node have been completed.
                        NodeQueriesComplete = true;
                        break;
                    }
                case NotificationType.ValueAdded:
                case NotificationType.ValueChanged:
                    {
                        // Added: A new node value has been added to OpenZWave's list. These notifications occur
                        // after a node has been discovered, and details of its command classes have been
                        // received. Each command class may generate one or more values depending on the
                        // complexity of the item being represented.
                        // Changed: A node value has been updated from the Z-Wave network and it is different from
                        // the previous value.
                        var value = notification.ValueID;
                        AddValue(value);
                        Debug.WriteLine($"{notification.Type}. Node {ID}: {ZWManager.Instance.GetValueLabel(value)} = {GetValue(value)} {ZWManager.Instance.GetValueUnits(value)}");
                        break;
                    }
                case NotificationType.ValueRemoved:
                    {
                        // A node value has been removed from OpenZWave's list. This only occurs when a
                        // node is removed.
                        // Note to self: We probably don't need to handle this, since the node would have been
                        // removed at this point
                        RemoveValue(notification.ValueID);
                        break;
                    }
                case NotificationType.Group:
                    {
                        // The associations for the node have changed.The application should rebuild any
                        // group information it holds about the node.
                        break;
                    }

                case NotificationType.Notification: //An error has occurred that we need to report.
                    {
                        Debug.WriteLine($"******Node error '{notification.Code}' @ ID: {ID}");
                        // var code = notification.Code;
                        // var v = GetValue(notification.ValueID);
                        break;
                    }
                case NotificationType.NodeEvent: // A node has triggered an event. This is commonly caused when a node sends a Basic_Set command to the controller. The event value is stored in the notification.
                    {
                        var value = GetValue(notification.ValueID);
                        Debug.WriteLine($"******Node Event @ ID: Value = {value}");
                        break;
                    }
                default:
                    {
                        Debug.WriteLine($"******Notification '{notification.Type}' not Handled @ ID: {ID}");
                        break;
                    }
            }
        }


        /// <summary>
        /// Helper method to get the string representation of a ZWValueID.
        /// </summary>
        /// <param name="v">The value</param>
        /// <returns></returns>
        private static string GetValue(ZWValueID v)
        {
            switch (v.Type)
            {
                case ZWValueType.Bool:
                    bool r1;
                    ZWManager.Instance.GetValueAsBool(v, out r1);
                    return r1.ToString();
                case ZWValueType.Byte:
                    byte r2;
                    ZWManager.Instance.GetValueAsByte(v, out r2);
                    return r2.ToString();
                case ZWValueType.Decimal:
                    string r3s;
                    ZWManager.Instance.GetValueAsString(v, out r3s);
                    return r3s;
                //throw new NotImplementedException("Decimal");
                //m_manager.GetValueAsDecimal(v, out r3);
                //return r3.ToString();
                case ZWValueType.Int:
                    Int32 r4;
                    ZWManager.Instance.GetValueAsInt(v, out r4);
                    return r4.ToString();
                case ZWValueType.List:
                    string r5;
                    ZWManager.Instance.GetValueListSelection(v, out r5);
                    return r5;
                case ZWValueType.Schedule:
                    return "Schedule";
                case ZWValueType.Short:
                    short r7;
                    ZWManager.Instance.GetValueAsShort(v, out r7);
                    return r7.ToString();
                case ZWValueType.String:
                    string r8;
                    ZWManager.Instance.GetValueAsString(v, out r8);
                    return r8;
                default:
                    return "";
            }
        }

        /// <summary>
        /// Fired if this node was removed from the controller. After this event has fired, do 
        /// not attempt to use this node again, but dispose of it.
        /// </summary>
        public event EventHandler NodeRemoved;

        internal void RaiseNodeRemoved()
        {
            NodeRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
