using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class WallMountMappingProfile : Profile
    {
        public WallMountMappingProfile()
        {
            CreateMap<Models.WallMountSwitch, WallMountSwitchDto>();

            CreateMap<WallMountSwitchDto, Models.WallMountSwitch>();
        }
    }
}
