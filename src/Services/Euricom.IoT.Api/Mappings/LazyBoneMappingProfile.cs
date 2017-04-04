using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Mappings
{
    public class LazyBoneMappingProfile : Profile
    {
        public LazyBoneMappingProfile()
        {
            CreateMap<LazyBone, LazyBoneDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Host));

            CreateMap<LazyBoneDto, LazyBone>()
                .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.Address));
        }
    }
}
