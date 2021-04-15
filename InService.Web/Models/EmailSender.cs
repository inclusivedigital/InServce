using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using InService.Data;
using InService.Lib.Auth;

namespace InService.Web.Models
{
    public class EmailSender
    {
        public SmtpClient SMTP { get; private set; }
        public EmailConfig Config { get; private set; }

        public EmailSender(EmailConfig config)
        {
            Config = config;
            SMTP = new SmtpClient(config.Host)
            {
                EnableSsl = config.EnableSSL
            };
            var Password = Config.Hash.GetPassword(config.Host.ToLower().Trim());
            if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(config.Username))
            {
                SMTP.UseDefaultCredentials = false;
                SMTP.Credentials = new NetworkCredential(Config.Username, Password);
            }
            if (Config.Port > 0) SMTP.Port = config.Port;
        }

        public EmailSender() => SMTP = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("mapikuw@gmail.com", "39051990@Tonny"),
            EnableSsl = true
        };

        public EmailSender(string Username, string Password, string Host, int Port) => SMTP = new SmtpClient(Host)
        {
            Port = Port,
            Credentials = new NetworkCredential(Username, Password),
            EnableSsl = true,
        };

        public async Task<Exception> SendEmail(string Subject, string Body, string To, params AlternateView[] alternateViews)
        {
            try
            {
                using (MailMessage message = new MailMessage
                {
                    IsBodyHtml = true,
                    Subject = Subject,
                    From = new MailAddress(Config != null ? Config.SenderID : "mapikuw@gmail.com"),
                })
                {
                    if (alternateViews?.Length > 0)
                    {
                        foreach (var item in alternateViews) message.AlternateViews.Add(item);
                    }
                    else message.Body = Body;
                    message.To.Add(To);
                    await SMTP.SendMailAsync(message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task BroadcastEmail(string From, string Subject, string Body, string To, params AlternateView[] alternateViews)
        {
            using (MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
                Subject = Subject,
                From = new MailAddress(From),
                Body = Body
            })
            {
                if (alternateViews != null)
                {
                    foreach (var item in alternateViews) message.AlternateViews.Add(item);
                }
                else message.Body = Body;
                message.To.Add(To);
                await SMTP.SendMailAsync(message);
            }
        }
    }
}