using System.Collections.Generic;
using Euricom.IoT.Models;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        void Remove(string username);
        User Get(string username);
        User GetUserWithToken(string token);
        List<User> Get();
        void Update(User user);
        bool Exists(string username);

        void AddUserRole(string username, string roleName);
        void RemoveUserRole(string username, string roleName);
        List<Role> GetRoles();

        void Seed();
    }
}