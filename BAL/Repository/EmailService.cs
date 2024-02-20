using BAL.Interface;

using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BAL.Repository
{
    public class EmailService : IEmailService
    {

     
    

        public async Task SendEmail(string to, string subject, string body)
        {

            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("", ""),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(""),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            smtpClient.Send(mailMessage);


        }

    }
}
