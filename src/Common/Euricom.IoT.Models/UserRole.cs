namespace Euricom.IoT.Models
{
    public class UserRole
    {
        public UserRole()
        {
            
        }

        public UserRole(string username, string roleName): this()
        {
            Username = username;
            RoleName = roleName;
        }

        public string Username { get; private set; }
        public User User { get; private set; }

        public string RoleName { get; private set; }
        public Role Role { get; private set; }
    }
}