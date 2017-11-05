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
            CreateMap<CameraDto, Camera>();
        }
    }
}
