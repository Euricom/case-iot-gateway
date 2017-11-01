using System;
using System.Collections.Generic;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Newtonsoft.Json;

namespace Euricom.IoT.DataLayer
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbBreezeDatabase _database;

        public UserRepository(IDbBreezeDatabase database)
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
                _database.SetValue(Constants.DBREEZE_TABLE_USERS, user.Username, user);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not set value for table: {Constants.DBREEZE_TABLE_USERS}, key: {user.Username}, exception: " + ex);
            }
        }

        public void Remove(string username)
        {
            try
            {
                using (var tran = _database.GetTransaction())
                {
                    tran.RemoveKey(Constants.DBREEZE_TABLE_USERS, username);
                    tran.Commit();
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
                return _database.GetValue<User>(Constants.DBREEZE_TABLE_USERS, username);
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
                var users = new List<User>();
                using (var tran = _database.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_USERS))
                    {
                        users.Add(JsonConvert.DeserializeObject<User>(row.Value));
                    }
                }
                return users;
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
                _database.SetValue(Constants.DBREEZE_TABLE_USERS, user.Username, user);
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
                using (var tran = _database.GetTransaction())
                {
                    var result = tran.Select<string, string>(Constants.DBREEZE_TABLE_USERS, username);
                    return result.Exists;
                }
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