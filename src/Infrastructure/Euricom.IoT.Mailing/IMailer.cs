namespace Euricom.IoT.Mailing
{
    public interface IMailer
    {
        void SendLostPasswordMail(string toEmailAddress, string password);
    }
}