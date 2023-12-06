using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace api_app
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            string fromMail = "babafemiayodele@gmail.com";
            string fromPassword = "rhlfugddrjrfmsvj";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(recipientEmail));
            message.Body = body;
            message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
                Timeout = 30 * 1000,
            };

            smtpClient.Send(message);
        }


    }

}