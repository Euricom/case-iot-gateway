using System.Collections.Generic;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IUserManager
    {
        void AddUser(UserDto userDto);
        void UpdateUser(UserDto userDto);
        void RemoveUser(string username);
        List<UserDto> GetUsers();
        UserDto GetUser(string username);

        List<RoleDto> GetRoles();

        string GenerateAccessToken(string username);
    }
}
