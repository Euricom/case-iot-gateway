using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.ZWave;

namespace Euricom.IoT.Api.Mappings
{
    public class NodeMappingProfile : Profile
    {
        public NodeMappingProfile()
        {
            CreateMap<Node, NodeDto>();
            CreateMap<NodeDto, Node>();
        }
    }
}