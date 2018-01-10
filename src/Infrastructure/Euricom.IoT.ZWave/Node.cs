using OpenZWave;
using System;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.ZWave
{
    public class Node: INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node(byte nodeId, uint homeId)
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
            get { return ZWManager.Instance.GetNodeName(HomeId, Id); }
            set
            {
                ZWManager.Instance.SetNodeName(HomeId, Id, value);
            }
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
    }
}
