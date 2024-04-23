using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JoreNoe.Message
{
    public interface IEmailMessageSettingConfigs
    {
        string SmtpUserName { get; set; }
        string SmtpHost { get; set; }
        int SmtpPort { get; set; }
        string Password { get; set; }
        bool EnableSSL { set; get; }
    }

    public class EmailMessageSettingConfigs : IEmailMessageSettingConfigs
    {
        public EmailMessageSettingConfigs() { }
        public EmailMessageSettingConfigs(string SmtpUserName,string SmtpHost,int SmtpPort,string Password,bool EnableSSL = false)
        {
            this.SmtpPort = SmtpPort;
            this.SmtpHost = SmtpHost;
            this.SmtpUserName = SmtpUserName;
            this.EnableSSL = EnableSSL;
            this.Password = Password;
        }



        public string SmtpUserName { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
    }

    /// <summary>
    /// 邮箱消息
    /// </summary>
    public interface IEmailMessageAPI
    {
        Task<bool> SendAsync(string ToEmailUser, string Subject, string Body, bool IsBodyHTML = false);
        bool Send(string ToEmailUser, string Subject, string Body, bool IsBodyHTML = false);
    }

    public class EmailMessageAPI : IEmailMessageAPI
    {
        public IEmailMessageSettingConfigs EmailMessageSettingConfigs { get; set; }
        public EmailMessageAPI(IEmailMessageSettingConfigs EmailMessageSettingConfigs)
        {
            this.EmailMessageSettingConfigs= EmailMessageSettingConfigs;
        }

        public bool Send(string ToEmailUser, string Subject, string Body, bool IsBodyHTML = false)
        {
            try
            {
                using (var smtpClient = new SmtpClient(this.EmailMessageSettingConfigs.SmtpHost, this.EmailMessageSettingConfigs.SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(this.EmailMessageSettingConfigs.SmtpUserName,this.EmailMessageSettingConfigs.Password);
                    smtpClient.EnableSsl = this.EmailMessageSettingConfigs.EnableSSL;

                    var message = new MailMessage
                    {
                        From = new MailAddress(this.EmailMessageSettingConfigs.SmtpUserName),
                        Subject = Subject,
                        Body = Body,
                        IsBodyHtml = IsBodyHTML
                    };
                    message.To.Add(ToEmailUser);

                    smtpClient.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendAsync(string ToEmailUser, string Subject, string Body, bool IsBodyHTML = false)
        {
            try
            {
                using (var smtpClient = new SmtpClient(this.EmailMessageSettingConfigs.SmtpHost, this.EmailMessageSettingConfigs.SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(this.EmailMessageSettingConfigs.SmtpUserName, this.EmailMessageSettingConfigs.Password);
                    smtpClient.EnableSsl = this.EmailMessageSettingConfigs.EnableSSL;
                    var message = new MailMessage
                    {
                        From = new MailAddress(this.EmailMessageSettingConfigs.SmtpUserName),
                        Subject = Subject,
                        Body = Body,
                        IsBodyHtml = IsBodyHTML
                    };
                    message.To.Add(ToEmailUser);

                    await smtpClient.SendMailAsync(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public static class JoreNoeEmailMessageExtensions 
    { 
        public static void AddJoreNoeEmailMessage(this IServiceCollection services, string SmtpUserName, string SmtpHost, int SmtpPort, string Password, bool EnableSSL = false)
        {
            services.AddSingleton<IEmailMessageSettingConfigs>(new EmailMessageSettingConfigs(SmtpUserName, SmtpHost, SmtpPort, Password, EnableSSL));
            services.AddScoped(typeof(IEmailMessageAPI), typeof(EmailMessageAPI));
        }
    }
}
