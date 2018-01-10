using System;
using System.Security.Cryptography;
using System.Text;

namespace Euricom.IoT.Models
{
    public class User
    {
        private static int _iterations = 5000;

        public User() { }

        public User(string username, string password)
        {
            Username = username;
            Salt = GenerateSalt();
            Hash = HashPassword(password, Salt, _iterations);
        }

        public string Username { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }

        public bool Check(string password)
        {
            return HashPassword(password, Salt, _iterations) == Hash;
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
            Hash = HashPassword(password, Salt, _iterations);
        }
    }
}