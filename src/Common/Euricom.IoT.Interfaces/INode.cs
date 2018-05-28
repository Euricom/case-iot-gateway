using System.Collections.Generic;

namespace Euricom.IoT.Interfaces
{
    public interface INode
    {
        byte Id { get; }
        byte GenericType { get; }
        byte SpecificType { get; }
        string Name { get; }
        string Label { get; }
        string Manufacturer { get; }
        string Product { get; }

        /// <summary>Gets the basic set of device values.</summary>
        List<INodeValue> BasicValues { get; }

        /// <summary>Gets the basic set of user values.</summary>
        List<INodeValue> UserValues { get;  }

        /// <summary>Gets the basic set of system values.</summary>
        List<INodeValue> SystemValues { get;  }

        /// <summary>Gets the basic set of configuration values.</summary>
        List<INodeValue> ConfigValues { get;  }
    }
}
