using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
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
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add user, key: {user.Username}, exception: " + ex);
            }
        }

        public void Remove(string username)
        {
            try
            {
                var user = _database.Users.Find(username);

                if (user != null)
                {
                    _database.Users.Remove(user);
                    _database.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public User Get(string username)
        {
            try
            {
                return _database
                    .Users
                    .Include(u => u.Roles)
                    .FirstOrDefault(u => u.Username == username);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public User GetUserWithToken(string token)
        {
            try
            {
                return _database.Users.FirstOrDefault(u => u.AccessToken == token);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public List<User> Get()
        {
            try
            {
                return _database.Users.Include(u => u.Roles).ToList();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public void Update(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                var u = _database.Users.Find(user.Username);

                _database.Entry(u).CurrentValues.SetValues(user);
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public bool Exists(string username)
        {
            try
            {
                return _database.Users.Find(username) != null;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public void AddUserRole(string username, string roleName)
        {
            var user = Get(username);
            if (user != null)
            {
                var role = GetRole(roleName);
                if (role == null)
                {
                    throw new Exception("Role not found.");
                }

                user.AddRole(role);
                _database.SaveChanges();
            }
        }

        public void RemoveUserRole(string username, string roleName)
        {
            var user = Get(username);
            if (user != null)
            {
                var role = GetRole(roleName);
                if (role == null)
                {
                    throw new Exception("Role not found.");
                }

                user.RemoveRole(roleName);
                _database.SaveChanges();
            }
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
            _database.Roles.Add(new Role(name));
            _database.SaveChanges();
        }

        private Role GetRole(string name)
        {
            return _database.Roles.FirstOrDefault(r => r.Name == name);
        }
    }
}