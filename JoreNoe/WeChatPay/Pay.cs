using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace WeChatListener
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("欢迎使用微信监听脚本...");

            try
            {
                var app = new WebApp();
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
        }
    }

    public class WebApp
    {
        private const string apiUrl = "http://v1.xxx.cn/api/recharge";
        private const string secretKey = "xxxxxxxxx";

        public async Task RunAsync()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:9000/");
            listener.Start();
            Console.WriteLine("监听微信消息中...");

            while (true)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "POST" && request.Url.LocalPath == "/msg")
                {
                    try
                    {
                        var content = await request.Content.ReadAsStringAsync();
                        await HandleMessageAsync(content);
                        response.StatusCode = 200;
                        response.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"处理消息时发生错误: {ex.Message}");
                        response.StatusCode = 500;
                        response.Close();
                    }
                }
                else
                {
                    response.StatusCode = 404;
                    response.Close();
                }
            }
        }

        private async Task HandleMessageAsync(string content)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            var isSelfMsg = xmlDoc.SelectSingleNode("/msg/is_self_msg").InnerText;
            var msgType = xmlDoc.SelectSingleNode("/msg/msg_type").InnerText;
            var wxId = xmlDoc.SelectSingleNode("/msg/wx_id").InnerText;

            if (isSelfMsg == "0" && msgType == "49" && (wxId == "notifymessage" || wxId == "gh_3dfda90e39d6"))
            {
                var match = Regex.Match(content, @"<title><!\[CDATA\[微信支付收款.+元.*]]></title>");
                if (match.Success)
                {
                    var amountNode = xmlDoc.SelectSingleNode(".//appmsg/mmreader/template_detail/line_content/topline/value/word");
                    var amount = amountNode != null ? amountNode.InnerText.Substring(1) : null;
                    Console.WriteLine($"收到金额：{amount}");

                    var desNode = xmlDoc.SelectSingleNode(".//appmsg/mmreader/template_detail/line_content/lines/line[key/word='付款方备注']/value/word");
                    var des = desNode?.InnerText;
                    Console.WriteLine($"备注信息：{des}");

                    if (Regex.IsMatch(des, @"^[\w\.-]+@[\w\.-]+\.\w+$"))
                    {
                        await RechargeAsync(des, amount);
                    }
                }
            }
        }

        private async Task RechargeAsync(string account, string amount)
        {
            Console.WriteLine($"尝试为“{account}”开通权限");

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("account", account),
                new KeyValuePair<string, string>("amount", amount),
                new KeyValuePair<string, string>("secret", secretKey)
            });

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(apiUrl, data);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("开通成功！");
                }
                else
                {
                    Console.WriteLine($"开通失败，原因：{result}");
                }
            }
        }
    }
}
