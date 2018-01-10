using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;

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
                return _database.Users.Find(username);
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
                return _database.Users.ToList();
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

        public void Seed()
        {
            if (!Exists("admin"))
            {
                Add(new User("admin", "secret_password"));
            }
        }
    }
}