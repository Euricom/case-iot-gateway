using OpenZWave;
using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;

namespace Euricom.IoT.ZWave
{
    public class Node : INode
    {
        private readonly List<ZWValueId> _userValues;
        private readonly List<ZWValueId> _configValues;
        private readonly List<ZWValueId> _basicValues;
        private readonly List<ZWValueId> _systemValues;

        private Node()
        {
            _userValues = new List<ZWValueId>();
            _configValues = new List<ZWValueId>();
            _basicValues = new List<ZWValueId>();
            _systemValues = new List<ZWValueId>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node(byte nodeId, uint homeId) : this()
        {
            Id = nodeId;
            HomeId = homeId;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public byte Id { get; }

        ///<summary>
        /// Gets or sets the home identifier.
        /// </summary>
        /// <value>
        /// The home identifier.
        /// </value>
        public UInt32 HomeId { get; }

        /// <summary>
        /// Gets or sets the name of the device. If the device supports it, this name is stored on the device.
        /// If not, it's only stored in the local device database, and could be lost if the database is lost or the app reset.
        /// </summary>
        public String Name
        {
            get => ZWManager.Instance.GetNodeName(HomeId, Id);
            set => ZWManager.Instance.SetNodeName(HomeId, Id, value);
        }

        /// <summary>
        /// Get a node's "generic" type.
        /// </summary>
        public byte GenericType => ZWManager.Instance.GetNodeGeneric(HomeId, Id);

        /// <summary>
        /// Get a node's "specific" type.
        /// </summary>
        public byte SpecificType => ZWManager.Instance.GetNodeSpecific(HomeId, Id);

        /// <summary>
        /// Gets a human-readable label describing the node.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public String Label => ZWManager.Instance.GetNodeType(HomeId, Id);

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public String Manufacturer => ZWManager.Instance.GetNodeManufacturerName(HomeId, Id);

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public String Product => ZWManager.Instance.GetNodeProductName(HomeId, Id);

        /// <summary>Gets the basic set of device values.</summary>
        public List<INodeValue> BasicValues => _basicValues.Select(Map).ToList();

        /// <summary>Gets the basic set of user values.</summary>
        public List<INodeValue> UserValues => _userValues.Select(Map).ToList();

        /// <summary>Gets the basic set of system values.</summary>
        public List<INodeValue> SystemValues => _systemValues.Select(Map).ToList();

        /// <summary>Gets the basic set of configuration values.</summary>
        public List<INodeValue> ConfigValues => _configValues.Select(Map).ToList();

        public void HandleEvent(ZWNotification notification)
        {
            switch (notification.Type)
            {
                case ZWNotificationType.ValueAdded:
                case ZWNotificationType.ValueRefreshed:
                case ZWNotificationType.ValueChanged:
                    {
                        // Added: A new node value has been added to OpenZWave's list. These notifications occur
                        // after a node has been discovered, and details of its command classes have been
                        // received. Each command class may generate one or more values depending on the
                        // complexity of the item being represented.
                        // Changed: A node value has been updated from the Z-Wave network and it is different from
                        // the previous value.
                        var value = AddValue(notification.ValueId);
                        Logger.Instance.Verbose($"{notification.Type}. Node {Id}: {value.Label} = {value.Value} {value.Unit}");

                        break;
                    }
                case ZWNotificationType.ValueRemoved:
                    {
                        // A node value has been removed from OpenZWave's list. This only occurs when a
                        // node is removed.
                        // Note to self: We probably don't need to handle this, since the node would have been
                        // removed at this point
                        RemoveValue(notification.ValueId);
                        break;
                    }
                case ZWNotificationType.Group:
                    {
                        // The associations for the node have changed.The application should rebuild any
                        // group information it holds about the node.
                        break;
                    }

                case ZWNotificationType.Notification: //An error has occurred that we need to report.
                    {
                        Logger.Instance.Verbose($"******Node error '{notification.Code}' @ ID: {Id}");
                        // var code = notification.Code;
                        // var v = GetValue(notification.ValueID);
                        break;
                    }
                case ZWNotificationType.NodeEvent
                    : // A node has triggered an event. This is commonly caused when a node sends a Basic_Set command to the controller. The event value is stored in the notification.
                    {
                        var value = GetValue(notification.ValueId);
                        Logger.Instance.Verbose($"******Node Event @ ID: Value = {value}");
                        break;
                    }
                default:
                    {
                        Logger.Instance.Verbose($"******Notification '{notification.Type}' not Handled @ ID: {Id}");
                        break;
                    }
            }
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="valueId">The value identifier.</param>
        private INodeValue AddValue(ZWValueId valueId)
        {
            IList<ZWValueId> list = GetValues(valueId.Genre);

            var id = list.FirstOrDefault(v => v.CommandClassId == valueId.CommandClassId && v.Id == valueId.Id);
            if (id != null)
            {
                list[list.IndexOf(id)] = valueId; //Update
            }
            else
            {
                list.Add(valueId); //New
            }

            return Map(valueId);
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="valueId">The value identifier.</param>
        private void RemoveValue(ZWValueId valueId)
        {
            var values = GetValues(valueId.Genre);
            values.Remove(valueId);
        }

        private INodeValue Map(ZWValueId valueId)
        {
            return new NodeValue
            {
                Id = valueId.Id,
                CommandClassId = valueId.CommandClassId,
                Index = valueId.Index,
                Instance = valueId.Instance,
                Type = Enum.GetName(typeof(ZWValueType), valueId.Type),
                Label = ZWManager.Instance.GetValueLabel(valueId),
                Value = GetValue(valueId),
                Unit = ZWManager.Instance.GetValueUnits(valueId)
            };
        }

        private IList<ZWValueId> GetValues(ZWValueGenre genre)
        {
            switch (genre)
            {
                case ZWValueGenre.Basic:
                    return _basicValues;
                case ZWValueGenre.Config:
                    return _configValues;
                case ZWValueGenre.System:
                    return _systemValues;
                case ZWValueGenre.User:
                    return _userValues;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Helper method to get the string representation of a ZWValueID.
        /// </summary>
        /// <param name="v">The value</param>
        /// <returns></returns>
        private static string GetValue(ZWValueId v)
        {
            switch (v.Type)
            {
                case ZWValueType.Bool:
                    ZWManager.Instance.GetValueAsBool(v, out var r1);
                    return r1.ToString();
                case ZWValueType.Byte:
                    ZWManager.Instance.GetValueAsByte(v, out var r2);
                    return r2.ToString();
                case ZWValueType.Decimal:
                    ZWManager.Instance.GetValueAsString(v, out var r3S);
                    return r3S;
                //throw new NotImplementedException("Decimal");
                //m_manager.GetValueAsDecimal(v, out r3);
                //return r3.ToString();
                case ZWValueType.Int:
                    ZWManager.Instance.GetValueAsInt(v, out var r4);
                    return r4.ToString();
                case ZWValueType.List:
                    ZWManager.Instance.GetValueListSelection(v, out string r5);
                    return r5;
                case ZWValueType.Schedule:
                    return "Schedule";
                case ZWValueType.Short:
                    ZWManager.Instance.GetValueAsShort(v, out var r7);
                    return r7.ToString();
                case ZWValueType.String:
                    ZWManager.Instance.GetValueAsString(v, out var r8);
                    return r8;
                default:
                    return "";
            }
        }

        public ZWValueId GetValueId(byte commandId, ZWValueGenre genre = ZWValueGenre.User)
        {
            List<ZWValueId> valueIds;
            switch (genre)
            {
                case ZWValueGenre.Basic:
                    valueIds = _basicValues;
                    break;
                case ZWValueGenre.User:
                    valueIds = _userValues;
                    break;
                case ZWValueGenre.Config:
                    valueIds = _configValues;
                    break;
                case ZWValueGenre.System:
                    valueIds = _systemValues;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(genre), genre, null);
            }

            return valueIds.FirstOrDefault(v => v.CommandClassId == commandId);
        }
    }
}
