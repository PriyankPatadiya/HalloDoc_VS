using BAL.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Net;
using System.Net.Mail;

namespace BAL.Repository
{
    public class EmailService : IEmailService
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public  void SendEmail(string to, string subject, string body)
        {

            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.priyankpatadiya@outlook.com", "priyanksoni@135"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.priyankpatadiya@outlook.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                
            };

            mailMessage.To.Add(to);

            smtpClient.Send(mailMessage);


        }

        public void SendMailWithAttachments(string to , string subject, string body , List<string> filePaths)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.priyankpatadiya@outlook.com", "priyanksoni@135"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.priyankpatadiya@outlook.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            foreach(var file in filePaths)
            {
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot\\uploads", file);
                var attachment = new Attachment(filePath);
                mailMessage.Attachments.Add(attachment);
            }

            mailMessage.To.Add(to);

            smtpClient.Send(mailMessage);
        }

    }
}
