using System;

namespace Euricom.IoT.Api.Models
{
    public class NodeDto
    {
        public byte Id { get; set; }
        public string GenericType { get; set; }
        public String Label { get; set; }
        public String Manufacturer { get; set; }
        public String Product { get; set; }
    }
}
