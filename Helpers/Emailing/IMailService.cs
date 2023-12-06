public interface IMailService
{
    bool SendMail(MailData mailData);
    Task<bool> SendMailAsync(MailData mailData);
}