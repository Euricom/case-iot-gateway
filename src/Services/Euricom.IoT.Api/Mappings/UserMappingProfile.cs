using System.Linq;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Mappings
{
    public class UserMappingProfile: Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Username, op => op.ResolveUsing(opts => opts.Username))
                .ForMember(d => d.AccessToken, op => op.ResolveUsing(opts => opts.AccessToken))
                .ForMember(d => d.Roles, op => op.ResolveUsing(r => r.Roles))
                .ForAllOtherMembers(expression => expression.Ignore());

            CreateMap<UserRole, string>().ConvertUsing(r => r.RoleName);

            CreateMap<Role, RoleDto>();
        }
    }
}
