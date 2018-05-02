using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Euricom.IoT.Models
{
    public class User
    {
        private const int Iterations = 5000;

        public User()
        {
            Roles = new List<UserRole>();
        }

        public User(string username, string password)
        {
            Username = username;
            Salt = GenerateSalt();
            Hash = HashPassword(password, Salt, Iterations);
            AccessToken = GenerateToken();
        }

        public string Username { get; private set; }
        public string Hash { get; private set; }
        public string Salt { get; private set; }
        public string AccessToken { get; private set; }

        public ICollection<UserRole> Roles { get; private set; }

        public bool Check(string password)
        {
            return HashPassword(password, Salt, Iterations) == Hash;
        }

        private static string GenerateSalt()
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                var random = new byte[32];
                generator.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        private static string HashPassword(string password, string salt, int iterations)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Convert.FromBase64String(salt), iterations))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(32));
            }
        }

        public void UpdatePassword(string password)
        {
            Salt = GenerateSalt();
            Hash = HashPassword(password, Salt, Iterations);
        }

        public string GenerateToken()
        {
            AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "");

            return AccessToken;
        }

        public void AddRole(Role role)
        {
            if (Roles.Any(r => r.RoleName == role.Name) == false)
            {
                Roles.Add(new UserRole(Username, role.Name));
            }
        }

        public void RemoveRole(string name)
        {
            var role = Roles.FirstOrDefault(r => r.RoleName == name);
            if (role != null)
            {
                Roles.Remove(role);
            }
        }
    }
}