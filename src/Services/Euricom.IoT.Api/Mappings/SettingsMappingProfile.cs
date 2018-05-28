using AutoMapper;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;
using System;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class SettingsMappingProfile : Profile
    {
        public SettingsMappingProfile()
        {
            CreateMap<SettingsDto, Settings>()
                .ForMember(d => d.LogLevel, op => op.ResolveUsing(opts => MapLogLevel(opts.LogLevel)))
                .ForMember(d => d.Id, src => src.Ignore())
                .ForMember(d => d.GatewayName, src => src.Ignore());

            CreateMap<Settings, SettingsDto>()
                .ForMember(d => d.LogLevel,
                    op => op.ResolveUsing(opts => Enum.GetName(typeof(LogLevel), opts.LogLevel)));
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
