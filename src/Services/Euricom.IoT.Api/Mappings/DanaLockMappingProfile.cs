using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class DanaLockMappingProfile : Profile
    {
        public DanaLockMappingProfile()
        {
            CreateMap<DanaLock, DanaLockDto>();
            CreateMap<DanaLockDto, DanaLock>()
                .ForMember(d => d.PrimaryKey, src => src.Ignore())
                .ForMember(d => d.Locked, src => src.Ignore())
                .ForMember(d => d.Type, src => src.Ignore());
        }
    }
}
