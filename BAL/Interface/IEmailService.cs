

namespace BAL.Interface
{
    public interface IEmailService
    {
        public void SendEmail(string to, string subject, string body);
        public void SendMailWithAttachments(string to, string subject, string body, List<string> filePaths);
    }
}
