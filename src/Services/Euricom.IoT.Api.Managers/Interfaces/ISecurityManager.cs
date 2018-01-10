namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ISecurityManager
    {
        string Login(string username, string password);
        string LoginWithPuk(string puk);
        string RequestCommandToken(string accessToken);
        bool ValidateToken(string jwt);
        void ChangePassword(string username, string old, string @new);
    }
}
