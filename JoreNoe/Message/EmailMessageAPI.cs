using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JoreNoe.Message
{
    // 邮箱API
    public class EmailMessageAPI
    {
        private string SmtpUserName;
        private string SmtpHost;
        private int SmtpPort;
        private string Password;
        private bool EnableSSL;

        public EmailMessageAPI()
        {

        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="smtpUserName">发送者</param>
        /// <param name="smtpHost">SMTP地址</param>
        /// <param name="smtpPort">SMTP端口</param>
        /// <param name="password">密码（这里的密码是 SMTP授权认证码）</param>
        /// <param name="enableSSL">是否开启SSL认证</param>
        public EmailMessageAPI(string smtpUserName, string smtpHost, int smtpPort, string password, bool enableSSL = false)
        {
            SmtpUserName = smtpUserName;
            SmtpHost = smtpHost;
            SmtpPort = smtpPort;
            Password = password;
            EnableSSL = enableSSL;
        }

        

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="smtpUserName">发送者</param>
        /// <param name="smtpHost">SMTP地址</param>
        /// <param name="smtpPort">SMTP端口</param>
        /// <param name="password">密码（这里的密码是 SMTP授权认证码）</param>
        /// <param name="enableSSL">是否开启SSL认证</param>
        public void RegisterEmail(string smtpUserName, string smtpHost, int smtpPort, string password, bool enableSSL = false)
        {
            SmtpUserName = smtpUserName;
            SmtpHost = smtpHost;
            SmtpPort = smtpPort;
            Password = password;
            EnableSSL = enableSSL;
        
        }

        /// <summary>
        /// 发送邮件短信
        /// </summary>
        /// <param name="ToEmailUser">接收人</param>
        /// <param name="Subject">标题</param>
        /// <param name="Body">主题内容</param>
        /// <param name="IsBodyHTML">是否开启兼容HTML</param>
        /// <returns></returns>
        public async Task<bool> SendAsync(string ToEmailUser, string Subject, string Body, bool IsBodyHTML = false)
        {
            try
            {
                using (var smtpClient = new SmtpClient(SmtpHost, SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(SmtpUserName, Password);
                    smtpClient.EnableSsl = EnableSSL;
                    var message = new MailMessage
                    {
                        From = new MailAddress(SmtpUserName),
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

        /// <summary>
        /// 发送邮件短信
        /// </summary>
        /// <param name="ToEmailUser">接收人</param>
        /// <param name="Subject">标题</param>
        /// <param name="Body">主题内容</param>
        /// <param name="IsBodyHTML">是否开启兼容HTML</param>
        /// <returns></returns>
        public bool Send(string ToEmailUser, string Subject, string Body, bool IsBodyHTML = false)
        {
            try
            {
                using (var smtpClient = new SmtpClient(SmtpHost, SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(SmtpUserName, Password);
                    smtpClient.EnableSsl = EnableSSL;

                    var message = new MailMessage
                    {
                        From = new MailAddress(SmtpUserName),
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
    }
}
