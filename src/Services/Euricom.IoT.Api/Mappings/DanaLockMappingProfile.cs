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
            CreateMap<DanaLockDto, DanaLock>();
        }
    }
}
