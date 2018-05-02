using Euricom.IoT.Security;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ISecurityManager
    {
        string Login(string username, string password);
        string LoginWithPuk(string puk);
        void ChangePassword(string username, string old, string @new);

        string RequestCommandToken(string token);
        bool ValidateToken(string jwt, out JwtSecurity.JwtPayload payload);
    }
}
