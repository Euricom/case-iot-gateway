using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class CameraMappingProfile : Profile
    {
        public CameraMappingProfile()
        {
            CreateMap<Camera, CameraDto>();

            CreateMap<CameraDto, Camera>()
                .ForMember(dest => dest.Type, opt => opt.UseValue<HardwareType>(HardwareType.Camera));
        }
    }
}
