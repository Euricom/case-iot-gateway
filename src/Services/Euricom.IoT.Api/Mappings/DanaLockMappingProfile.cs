using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class DanaLockMappingProfile : Profile
    {
        public DanaLockMappingProfile()
        {
            CreateMap<DanaLock, DanaLockDto>();

            CreateMap<DanaLockDto, DanaLock>()
                .ForMember(dest => dest.Type, opt => opt.UseValue<HardwareType>(HardwareType.DanaLock));
        }
    }
}
