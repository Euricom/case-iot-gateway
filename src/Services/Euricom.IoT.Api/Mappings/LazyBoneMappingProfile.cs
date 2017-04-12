using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class LazyBoneMappingProfile : Profile
    {
        public LazyBoneMappingProfile()
        {
            CreateMap<LazyBone, LazyBoneDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Host));

            CreateMap<LazyBoneDto, LazyBone>()
                .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Type, opt => opt.UseValue<HardwareType>(HardwareType.LazyBoneSwitch));
        }
    }
}
