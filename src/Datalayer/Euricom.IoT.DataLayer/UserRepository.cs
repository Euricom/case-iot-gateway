using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;
using Microsoft.EntityFrameworkCore;

namespace Euricom.IoT.DataLayer
{
    public class UserRepository : IUserRepository
    {
        private readonly IotDbContext _database;

        public UserRepository(IotDbContext database)
        {
            _database = database;
        }

        public void Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                _database.Users.Add(user);
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.HandleAlreadyExistsException();
                throw;
            }
        }

        public void Remove(string username)
        {
            var user = _database
                .Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                _database.Users.Remove(user);
                _database.SaveChanges();
            }
        }

        public User Get(string username)
        {
            var user = _database
                .Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                throw new NotFoundException(username);
            }

            return user;
        }

        public User GetUserWithToken(string token)
        {
            var user = _database.Users.FirstOrDefault(u => u.AccessToken == token);

            if (user == null)
            {
                throw new NotFoundException(token);
            }

            return user;
        }
        
        public List<User> Get()
        {
            return _database.Users.Include(u => u.Roles).ToList();
        }

        public void Update(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var u = Get(user.Username);

            _database.Entry(u).CurrentValues.SetValues(user);
            _database.SaveChanges();
        }

        public bool Exists(string username)
        {
            return _database
                .Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username == username) != null;
        }

        public void AddUserRole(string username, string roleName)
        {
            var user = Get(username);
            var role = GetRole(roleName);

            user.AddRole(role);

            _database.SaveChanges();
        }

        public void RemoveUserRole(string username, string roleName)
        {
            var user = Get(username);
            var role = GetRole(roleName);

            user.RemoveRole(role.Name);

            _database.SaveChanges();
        }

        public List<Role> GetRoles()
        {
            return _database.Roles.ToList();
        }

        public void Seed()
        {
            if (!Exists("admin"))
            {
                Add(new User("admin", "secret_password"));
            }

            if (GetRole("Administrator") == null)
            {
                AddRole("Administrator");
            }

            if (GetRole("Manager") == null)
            {
                AddRole("Manager");
            }

            if (GetRole("User") == null)
            {
                AddRole("User");
            }
        }

        private void AddRole(string name)
        {
            try
            {
                _database.Roles.Add(new Role(name));
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.HandleAlreadyExistsException();
                throw;
            }
        }

        private Role GetRole(string name)
        {
            var role = _database.Roles.FirstOrDefault(r => r.Name == name);

            if (role == null)
            {
                throw new NotFoundException(name);
            }

            return role;
        }
    }
}