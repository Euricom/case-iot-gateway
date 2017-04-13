namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ISecurityManager
    {
        string Login(string username, string password);
        string LoginWithPUK(string PUK);
        string RequestCommandToken(string accessToken);
        bool ValidateToken(string jwt);
        void LostPassword();
        void ResetPassword(string resetGuid);
    }
}
