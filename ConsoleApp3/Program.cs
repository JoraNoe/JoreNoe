using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Net.Http;

namespace ConsoleApp3
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            JoreNoe.Message.EmailMessageAPI message = 
                new JoreNoe.Message.EmailMessageAPI("jorenoe@yeah.net","smtp.yeah.net",25, "WSMWUPMFXDXIAGAR");

            await message.SendAsync("jth_net@163.com","你好牛米好","库你洗哇",true);

            //for (int i = 0; i < 10000; i++)
            //{
            //    foreach (string s in new string[] { "jth_net@163.com", "319116952@qq.com" })
            //    {
            //        await SendEmailAsync(s, "哈哈哈", "我爱你").ConfigureAwait(false);
            //    }

            //    await Task.Delay(3000);
            //}
        }

        public static async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            try
            {
                if (!string.IsNullOrEmpty(toAddress))
                {
                    string smtpUsername = "jorenoe@yeah.net";
                    using (var client = new SmtpClient("smtp.yeah.net", 25))
                    {
                        //WSMWUPMFXDXIAGAR 
                        //BTYHGUSKPRHUZWFG
                        client.Credentials = new NetworkCredential(smtpUsername, "WSMWUPMFXDXIAGAR");
                        client.EnableSsl = false;
                        var message = new MailMessage
                        {

                            From = new MailAddress(smtpUsername),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        };
                        message.To.Add(toAddress);
                        await client.SendMailAsync(message);
                    }

                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="message"></param>
        public static void SendEmailFor(string message)
        {
            try
            {
                Task.Run(async () =>
                {
                    foreach (var item in new string[] { "jth_net@163.com", "1021525382@qq.com", "183815867@qq.com" })
                    {
                        await SendEmailAsync(item, "线上全网博思开票出问题提醒", $@"
                        <!DOCTYPE html>
                            <html lang=""en"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                            </head>
                            <body style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;"">

                                <div style=""max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 15px rgba(0, 0, 0, 0.1); text-align: center;"">

                                    <h2 style=""color: #007BFF; margin-bottom: 20px;"">线上发票提醒</h2>
                                    <P style=""color: #333333; line-height: 1.6; text-align: justify;"">开票时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</p>
                                    <p style=""color: #333333; line-height: 1.6; text-align: justify;"">开票产生的问题：{message}</p>

                                </div>

                            </body>
                            </html>

                        ");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送失败");
            }
        }


        static async Task ok(string[] oks) // 限制并发数为1)
        {

            await Task.Run(async () =>
            {
                // 消费  
                foreach (string s in oks)
                {
                    okk(s);
                }
            }).ConfigureAwait(false);



        }

        static void okk(string okss)
        {
            // fasong 

            Console.WriteLine(okss);




        }
    }
}
