
using Dapper.Contrib.Extensions;
using JoreNoe.DB.Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NPOI.Util;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using JoreNoe.WeChatPay.WeChatListener;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using System.Xml;
using System.Text.RegularExpressions;
using JoreNoe.JoreHttpClient;
using System.Net.Http;
using System.Text;

namespace Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //// 获取当前时间
            //DateTime currentTime = DateTime.Now;

            //// 假设有另一个时间点 targetTime
            //DateTime targetTime = DateTime.Parse("2023-10-27 14:30:00");

            //// 计算时间差
            //TimeSpan timeDifference = targetTime - currentTime;

            //// 获取时间差的小时数
            //double hoursDifference = timeDifference.TotalHours;

            //Console.WriteLine("当前时间与目标时间相差：" + hoursDifference + " 小时");

            //caonima();
            //check();

            //testExists();

            //ment();

            //TestSoftRemove();

            //
            //calc();
            //await TestInsert();

            // await TestInsert();
            //caonimade();

            //MakePostRequest("asdf", "https://api.douban.com/v2/movie/top250");
            //ment1();        

            // test111();
            //for (int i = 0;i<100000;i++)
            //{
            //    Sendemail();
            //    await Task.Delay(1000);
            //}
            //Sendemail();

            //Parallel.For(0, 10000, i =>
            //{
            //    var List = new List<testClass>() {
            //        new testClass{ Name="张三",Time = DateTime.Parse("2023-11-12 11:11:20") },
            //        new testClass{ Name="帅哥",Time = DateTime.Parse("2024-11-12 11:11:20") }
            //    };

            //    if(List.Any())
            //    {
            //        var Laster = List.OrderByDescending(d => d.Time).FirstOrDefault();
            //        List.Clear();
            //        List.Add(Laster);

            //        Console.WriteLine(List.First().Name + " -- " + List.Count);
            //    }


            //});

            Console.WriteLine("欢迎使用微信监听脚本...");

            //try
            //{
            //    var app = new WebApp();
            //    await app.RunAsync();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"发生错误: {ex.Message}");
            //}

            //mentsaf s = new mentsaf();

            //s.ok();

            hhhh();



            Console.ReadKey();
        }


        public static async Task hhhh()
        {
            var postData = new
            {
                Account = "123412",
                Remark = "1234",
                Amount = "1234333"
            };

            // 将数据转换为 JSON 字符串
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);

            // 设置要发送的请求地址
            string url = "https://jorenoe.top/api/JoreNoePay/Notice";

            // 创建 HttpClient 实例
            using var httpClient = new HttpClient();

            // 创建要发送的 HTTP 请求内容
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // 发送 HTTP POST 请求
                var response = await httpClient.PostAsync(url, content);

                // 检查响应是否成功
                if (response.IsSuccessStatusCode)
                {
                    // 处理成功响应
                    var responseData = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // 处理失败响应
                }
            }
            catch (Exception ex)
            {

            }
        }


        public class mentsaf {
        
            public async Task ok()
            {
                Console.WriteLine("欢迎使用微信消息监听脚本...");

                try
                {
                    var listener = new HttpListener();
                    listener.Prefixes.Add("http://127.0.0.1:9000/");
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
                                var content = await new System.IO.StreamReader(request.InputStream).ReadToEndAsync();

                                var json = JObject.Parse(content);
                                var messageContent = (string)json["content"];

                                string title = mentsaf.GetAmountFromTitle(messageContent);
                                string des = mentsaf.GetRemarkFromDes(messageContent);

                                string mm = mentsaf.GetRemarkFromDes1(messageContent);

                                Console.WriteLine("Amount: " + title);
                                Console.WriteLine("Remark: " + des);
                                Console.WriteLine("Remark: " + mm);

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
                catch (Exception ex)
                {
                    Console.WriteLine($"发生错误: {ex.Message}");
                }
            }

            public static string GetRemarkFromDes1(string xml)
            {
                string pattern = @"汇总([\s\S]*?)备注收款成功";
                Match match = Regex.Match(xml, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
                return string.Empty;
            }

            public  static string GetAmountFromTitle(string xml)
            {
                string pattern = @"微信支付收款(\d+(\.\d+)?)元";
                Match match = Regex.Match(xml, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return string.Empty;
            }

            // 从描述中提取备注的方法
            public static string GetRemarkFromDes(string xml)
            {
                string pattern = @"付款方备注([^\n]*)";
                Match match = Regex.Match(xml, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
                return string.Empty;
            }
            private static async Task HandleMessageAsync(string content)
            {
                var json = JObject.Parse(content);
                var messageType = (int)json["msg_type"];
                var messageContent = (string)json["content"];

                switch (messageType)
                {
                    case 1:
                        Console.WriteLine($"收到文本消息: {messageContent}");
                        break;
                    case 3:
                        Console.WriteLine($"收到图片消息: {messageContent}");
                        break;
                    case 34:
                        Console.WriteLine($"收到语音消息: {messageContent}");
                        break;
                    // 处理其他消息类型...
                    default:
                        Console.WriteLine($"收到未知消息类型({messageType}): {messageContent}");
                        break;
                }
            }

        }





        public class testClass
        {
            public string Name { get; set; }
            public DateTime Time { get; set; }
        }

        public async static void Sendemail()
        {

            JoreNoe.Message.EmailMessageAPI message =
                new JoreNoe.Message.EmailMessageAPI("postmaster@jorenoe.top", "smtp.qiye.aliyun.com", 25, "zw.047600");

            await message.SendAsync("jth_net@163.com", "登录验证码", "验证码：98782783", true);

        }
        public static void test111()
        {
            var ment = new List<string>();
            ment.Add("asd");
            Console.WriteLine(ment.Any());
        }

        public static void ment1()
        {
            var list1 = new List<int>();
            var list2 = new List<int>();

            list1.Add(1);
            list1.Add(2);

            list2.Add(2);

            list1 = list1.Where(d => !list2.Select(s => s).Contains(d)).ToList();
            Console.WriteLine(list1);
        }

        public static string MakePostRequest(string Body, string RequestUrl)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(RequestUrl);
            httpWebRequest.ContentType = "application/json;charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 15000;
            httpWebRequest.ReadWriteTimeout = 15000;
            //判断是否存在Body 数据
            if (!string.IsNullOrEmpty(Body))
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(Body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            string respContent = "";

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        respContent = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }


            return respContent == null ? string.Empty : respContent;
        }

        static void caonimade()
        {
            Console.WriteLine(Guid.NewGuid().ToString("N"));
            Console.WriteLine(Guid.NewGuid());
        }

        static void calc()
        {
            for (int i = 1; i <= 60; i++)
            {
                Console.WriteLine((i + (i % 30 == 0).ToString()));
            }
        }
        static void ment()
        {
            var ments = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                ments.Add(i);
            }

            var x = new ConcurrentQueue<string>();

            Parallel.ForEach(ments, d =>
            {

                foreach (var item in ments)
                {
                    x.Enqueue(item + "");
                }

            });


        }

        static void check()
        {
            var list1 = new List<string>();
            list1.Add("3");

            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(i + string.Empty);
            }

            var x = list.Where(d => !list1.Contains(d));

        }

        static void caonima()
        {
            var Latter = "\r\n";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, Dictionary<string, string>>
                {
                    {
                        "thing1", new Dictionary<string, string>
                        {
                            {"value", $"{"你好我是课程"}{Latter}{Latter}"}
                        }
                    },
                    {
                        "time2", new Dictionary<string, string>
                        {
                            {"value", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}{Latter}{Latter}"}
                        }
                    },
                    {
                        "time4", new Dictionary<string, string>
                        {
                            {"value", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}{Latter}{Latter}"}
                        }
                    }
                }
            );

        }

        static void testExists()
        {
            Registory.SetInitDbContext("Server=119.3.208.175;Database=ouconline_allinone_datacenter;User ID=ouconline_allinone_datacenter;Password=zeNnwfyD5ue2z81V;", IDBType.MySql);
            var database = new Repository<Employees>();

            var x = database.IsExists(12000340, "employee_id");

        }

        static void TestSoftRemove()
        {
            Registory.SetInitDbContext("Server=119.3.208.175;Database=ouconline_allinone_datacenter;User ID=ouconline_allinone_datacenter;Password=zeNnwfyD5ue2z81V;", IDBType.MySql);
            var database = new Repository<Employees>();

            database.SoftRemove(1234, "employee_id", "ceshi", false);

        }

        // 测试插入数据 批量 高性能
        static async Task TestInsert()
        {
            //Registory.SetInitDbContext("Server=mysql.sqlpub.com;Database=mydbcloud;User Id=jorenoe;Password=48db25c68757687a;", IDBType.MySql);

            Registory.SetInitDbContext("Server=100.88.44.122;Database=jorenoe;User ID=root;Password=JoreNoe123$%^;", IDBType.MySql);
            var database = new Repository<Employees>();



            for (int s = 0; s < 200000000000; s++)
            {

                var lists = new ConcurrentQueue<Employees>();
                for (int i = 0; i < 2000; i++)
                {
                    lists.Enqueue(new Employees
                    {
                        employee_id = Guid.NewGuid().ToString().Replace("-", ""),
                        first_name = "asdf",
                        last_name = "123456",
                        hire_date = DateTime.Now,
                        email = "12@qq.com",
                        ceshi = true,
                        id_number = "123",
                        phone_number = "12341234",
                        verification_code = "12341"
                    });
                    // Console.WriteLine(i);
                }



                // 创建 Stopwatch 对象
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                database.BulkInsert(lists);
                // 停止计时
                stopwatch1.Stop();
                // 获取经过的时间
                TimeSpan elapsed1 = stopwatch1.Elapsed;

                //输出经过的时间
                Console.WriteLine($"封装2经过的时间: {elapsed1}");
            }




            //for (int i = 0; i < 10; i++)
            //{
            //    //database.Removes<string>(lists.Select(d => d.employee_id.ToString()).ToArray(), "employee_id");
            //    // 创建 Stopwatch 对象
            //    Stopwatch stopwatch1 = new Stopwatch();
            //    stopwatch1.Start();
            //    database.BulkInsert(lists);
            //    // 停止计时
            //    stopwatch1.Stop();
            //    // 获取经过的时间
            //    TimeSpan elapsed1 = stopwatch1.Elapsed;

            //    // 输出经过的时间
            //    Console.WriteLine($"封装经过的时间: {elapsed1}");
            //}
        }

    }
    // 测试实体
    public class Employees
    {
        public string employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        public string email { get; set; }

        public DateTime hire_date { get; set; }

        public bool ceshi { get; set; }

        public string phone_number { set; get; }

        public string id_number { set; get; }

        public string verification_code { get; set; }
    }
}
