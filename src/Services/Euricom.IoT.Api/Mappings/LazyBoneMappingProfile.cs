using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.LazyBone;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class LazyBoneMappingProfile : Profile
    {
        public LazyBoneMappingProfile()
        {
            CreateMap<LazyBone, LazyBoneDto>();

            CreateMap<LazyBoneDto, LazyBone>()
                .ForMember(dest => dest.Type, opt => opt.UseValue<HardwareType>(HardwareType.LazyBoneSwitch))
                .ForMember(d => d.PrimaryKey, src => src.Ignore())
                .ForMember(d => d.Type, src => src.Ignore());
        }
    }
}
