using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _repository;

        public UserManager(IUserRepository repository)
        {
            _repository = repository;
        }

        public void AddUser(UserDto userDto)
        {
            var user = new User(userDto.Username, "Euri123com!");

            _repository.Add(user);

            if (userDto.Roles != null)
            {
                foreach (var role in userDto.Roles)
                {
                    _repository.AddUserRole(user.Username, role);
                }
            }
        }

        public void UpdateUser(UserDto userDto)
        {
            if (userDto.Username == "admin")
            {
                throw new NotAllowedException();
            }

            var user = _repository.Get(userDto.Username);

            var delete = user
                .Roles
                .Select(r => r.RoleName)
                .Except(userDto.Roles ?? new List<string>())
                .ToList();

            foreach (var role in delete)
            {
                _repository.RemoveUserRole(user.Username, role);
            }

            foreach (var role in userDto.Roles ?? new List<string>())
            {
                _repository.AddUserRole(user.Username, role);
            }
        }

        public void RemoveUser(string username)
        {
            if (username == "admin")
            {
                throw new NotAllowedException();
            }

            _repository.Remove(username);
        }

        public List<UserDto> GetUsers()
        {
            var users = _repository.Get();
            return Mapper.Map<List<UserDto>>(users);
        }

        public UserDto GetUser(string username)
        {
            var user = _repository.Get(username);
            return Mapper.Map<UserDto>(user);
        }

        public List<RoleDto> GetRoles()
        {
            var roles = _repository.GetRoles();
            return Mapper.Map<List<RoleDto>>(roles);
        }

        public string GenerateAccessToken(string username)
        {
            var user = _repository.Get(username);
            user.GenerateToken();

            _repository.Update(user);

            return user.AccessToken;
        }
    }
}
