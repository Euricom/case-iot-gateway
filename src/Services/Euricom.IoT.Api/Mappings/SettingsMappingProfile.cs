using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;
using System;

namespace Euricom.IoT.Api.Mappings
{
    public class SettingsMappingProfile : Profile
    {
        public SettingsMappingProfile()
        {
            CreateMap<SettingsDto, Settings>()
                .ForMember(d => d.LogLevel, op => op.ResolveUsing(opts => MapLogLevel(opts.LogLevel)))
                .ReverseMap();

            CreateMap<Settings, SettingsDto>()
               .ForMember(d => d.LogLevel, op => op.ResolveUsing(opts => opts.LogLevel.ToString()))
               .ReverseMap();
        }

        private LogLevel MapLogLevel(string logLevel)
        {
            try
            {
                return (LogLevel)Enum.Parse(typeof(LogLevel), logLevel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
