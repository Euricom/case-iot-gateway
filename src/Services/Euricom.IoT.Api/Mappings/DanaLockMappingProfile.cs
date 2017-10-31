﻿using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class DanaLockMappingProfile : Profile
    {
        public DanaLockMappingProfile()
        {
            CreateMap<IoT.Models.DanaLock, DanaLockDto>();

            CreateMap<DanaLockDto, IoT.Models.DanaLock>()
                .ForMember(dest => dest.Type, opt => opt.UseValue<HardwareType>(HardwareType.DanaLock));
        }
    }
}
