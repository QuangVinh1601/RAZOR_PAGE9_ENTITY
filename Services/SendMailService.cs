using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;
using Microsoft.VisualStudio.Web.CodeGeneration;
using MimeKit;

namespace RAZOR_PAGE9_ENTITY.Services
{
    public class MailSetting
    {
        public string Mail { set; get; }
        public string DisplayName { set; get; }
        public string Password { set; get; }
        public string Host { set; get; }
        public int Port { set; get; }
    }
    public class SendMailService : IEmailSender
    {
        private readonly MailSetting mailSettings;
        private readonly ILogger<SendMailService> logger;
        public SendMailService(IOptions<MailSetting> setting, ILogger<SendMailService> _logger)
        {
            mailSettings = setting.Value;
            logger = _logger;
            logger.LogInformation("Create SendMailService");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                //Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);

                logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                logger.LogError(ex.Message);

            }

            smtp.Disconnect(true);

            logger.LogInformation("send mail to " + email);
        }
    }
}
