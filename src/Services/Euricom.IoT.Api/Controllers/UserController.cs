using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Administrator")]
    [RestController]
    public class UserController: ControllerBase
    {
        private readonly IUserManager _manager;

        public UserController(IUserManager manager)
        {
            _manager = manager;
        }

        [UriFormat("/users")]
        public IGetResponse GetUsers()
        {
            var users = _manager.GetUsers();
            return ResponseUtilities.GetResponseOk(users);
        }

        [UriFormat("/users/me")]
        public IGetResponse GetUser()
        {
            var users = _manager.GetUser(GetUsername());
            return ResponseUtilities.GetResponseOk(users);
        }

        [UriFormat("/users")]
        public IPostResponse AddUser([FromContent] UserDto userDto)
        {
            _manager.AddUser(userDto);

            var user = _manager.GetUser(userDto.Username);
            return ResponseUtilities.PostResponseOk(user);
        }

        [UriFormat("/users")]
        public IPutResponse UpdateUser([FromContent] UserDto userDto)
        {
            _manager.UpdateUser(userDto);

            var user = _manager.GetUser(userDto.Username);
            return ResponseUtilities.PutResponseOk(user);
        }

        [UriFormat("/users/{username}")]
        public IDeleteResponse RemoveUser(string username)
        {
            _manager.RemoveUser(username);

            return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
        }

        [UriFormat("/users/roles")]
        public IGetResponse GetRoles()
        {
            var roles = _manager.GetRoles();
            return ResponseUtilities.GetResponseOk(roles);
        }

        [UriFormat("/users/{username}/access-token")]
        public IPutResponse GenerateAccessToken(string username)
        {
            var token = _manager.GenerateAccessToken(username);
            return ResponseUtilities.PutResponseOk(token);
        }
    }
}
