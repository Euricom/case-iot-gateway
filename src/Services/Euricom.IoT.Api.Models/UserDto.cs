using System.Collections.Generic;

namespace Euricom.IoT.Api.Models
{
    public class UserDto
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }

        public List<string> Roles { get; set; }
    }
}