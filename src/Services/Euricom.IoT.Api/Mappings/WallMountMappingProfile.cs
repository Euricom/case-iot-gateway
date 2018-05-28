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
            CreateMap<WallMountSwitchDto, WallMountSwitch>()
                .ForMember(d => d.PrimaryKey, src => src.Ignore())
                .ForMember(d => d.On, src => src.Ignore())
                .ForMember(d => d.Type, src => src.Ignore());
        }
    }
}
