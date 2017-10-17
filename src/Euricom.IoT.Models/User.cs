using System.Security.Cryptography;
using System.Text;

namespace Euricom.IoT.Models
{
    public class User
    {
        private static int _iterations = 5000;

        public User(string username, string password)
        {
            Username = username;
            Salt = GenerateSalt();
            Hash = HashPassword(password, Salt, _iterations);
        }

        public string Username { get; }
        public byte[] Hash { get; private set; }
        public byte[] Salt { get; private set; }

        public bool Check(string password)
        {
            return HashPassword(password, Salt, _iterations) == Hash;
        }

        private static byte[] GenerateSalt()
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                var random = new byte[32];
                generator.GetBytes(random);

                return random;
            }
        }
        private static byte[] HashPassword(string password, byte[] salt, int iterations)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, iterations))
            {
                return rfc2898DeriveBytes.GetBytes(32);
            }
        }
    }
}