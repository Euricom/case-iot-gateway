using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.Camera;

namespace Euricom.IoT.Api.Mappings
{
    public class CameraMappingProfile : Profile
    {
        public CameraMappingProfile()
        {
            CreateMap<Camera, CameraDto>();
            CreateMap<CameraDto, Camera>()
                .ForMember(d => d.PrimaryKey, src => src.Ignore())
                .ForMember(d => d.Type, src => src.Ignore());
        }
    }
}
