﻿using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.Api.Mappings
{
    public class LogMappingProfile : Profile
    {
        public LogMappingProfile()
        {
            CreateMap<Log, LogDto>()
                .ReverseMap();

            CreateMap<LogLine, LogLineDto>()
                .ReverseMap();
        }
    }
}
