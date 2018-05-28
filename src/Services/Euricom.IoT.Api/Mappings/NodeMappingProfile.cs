using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Api.Mappings
{
    public class NodeMappingProfile : Profile
    {
        public NodeMappingProfile()
        {
            CreateMap<INode, NodeDto>();
        }
    }
}