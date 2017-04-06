using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Common;

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

            CreateMap<LogLineDto, LogLine>()
                .ForMember(dest => dest.Properties, opt => opt.Ignore());

            CreateMap<LogLine, LogLineDto>();
        }
    }
}
