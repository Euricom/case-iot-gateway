using System.Collections.Generic;

namespace Euricom.IoT.Api.Models
{
    public class NodeDto
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public byte GenericType { get; set; }
        public string Label { get; set; }
        public string Manufacturer { get; set; }
        public string Product { get; set; }

        /// <summary>Gets the basic set of device values.</summary>
        public List<ValueDto> BasicValues { get; set; }

        /// <summary>Gets the basic set of user values.</summary>
        public List<ValueDto> UserValues { get; set; }

        /// <summary>Gets the basic set of system values.</summary>
        public List<ValueDto> SystemValues { get; set; }

        /// <summary>Gets the basic set of configuration values.</summary>
        public List<ValueDto> ConfigValues { get; set; }
    }
}
