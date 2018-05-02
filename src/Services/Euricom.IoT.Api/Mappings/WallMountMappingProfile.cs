using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.WallMountSwitch;

namespace Euricom.IoT.Api.Mappings
{
    public class WallMountMappingProfile : Profile
    {
        public WallMountMappingProfile()
        {
            CreateMap<WallMountSwitch, WallMountSwitchDto>();
            CreateMap<WallMountSwitchDto, WallMountSwitch>();
        }
    }
}
