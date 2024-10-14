using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using JoreNoe.Queue.RBMQ;
using Microsoft.Extensions.DependencyInjection;
using JoreNoe.Cache.Redis;
using AutoMapper;
using JoreNoe.Extend;
using System.Security.Cryptography;
using JoreNoe.DB.Dapper;
using System.Diagnostics;
using Org.BouncyCastle.Bcpg.Sig;
using NPOI.SS.Formula.Functions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JoreNoe.JoreHttpClient;

namespace ConsoleApp1
{

    public class test
    {
        public string Name { get; set; }
        public bool Flg { get; set; }
        public string Email { get; set; }
    }
    public class test1
    {
        public string name { get; set; }
        public int age { get; set; }

        public DateTime time { get; set; }

        public string OrganizationId { get; set; }
    }



    //public class PhoneStore : ICustome<test>
    //{
    //    public async Task<test> ConSume(CustomeContent<test> Context)
    //    {
    //        await Console.Out.WriteLineAsync(Context.Context.name);
    //        return null;
    //    }
    //}


    public static class m
    {
        public static string CalculateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }

    internal class Program
    {



        static async Task Main(string[] args)
        {

            //var count = 20;
            //var list = new List<string>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    list.Add(i.ToString());
            //}

            //var ok = list.YieldCollectionList(100);


            //var x = m.CalculateMD5Hash("2180201202515" + "60a96e6d-5db0-4bfc-b7c5-9e2ebe3815b6");



            //var ment = "66701-[{\"Key\":\"WeChatData\",\"Value\":\"{\\\"NewsData\\\":null,\\\"OpenId\\\":null,\\\"MessageType\\\":\\\"Template\\\",\\\"MessageContext\\\":null,\\\"Title\\\":\\\"课表提醒\\\",\\\"Desc\\\":\\\"您在2024/05/09 13:00将有一节习近平新时代中国特色社会主义思想概论直播课，请按时参加\\\\r\\\\n\\\",\\\"UrlAddress\\\":\\\"http://student.allinone.ouc-online.com.cn//#/liveroomtransfer/63cb8e1f-4727-4058-aea9-b05b2941fa1c\\\",\\\"ModuleData\\\":{\\\"first\\\":{\\\"value\\\":\\\"您在2024/05/09 13:00将有一节《习近平新时代中国特色社会主义思想概论》直播课，请按时参加\\\\r\\\\n\\\"},\\\"keyword1\\\":{\\\"value\\\":\\\"《习近平新时代中国特色社会主义思想概论》\\\\r\\\\n\\\"},\\\"keyword2\\\":{\\\"value\\\":\\\"2024-05-09 13:00\\\\r\\\\n\\\"},\\\"remark\\\":{\\\"value\\\":\\\"请按时上课\\\"}},\\\"TopColor\\\":\\\"#7B68EE\\\",\\\"TemplateId\\\":\\\"s11Vy-9bbxuhywd9LW_Brp7SXUgggt-U93sADCp2JzU\\\",\\\"Message\\\":null}\"},{\"Key\":\"ClassTime\",\"Value\":\"2024-05-09 13:00\"},{\"Key\":\"CourseName\",\"Value\":\"习近平新时代中国特色社会主义思想概论\"}]";

            //var Data = new ConcurrentBag<test>();
            //Parallel.For(0, 1000000, e =>
            //{
            //    Data.Add(new test
            //    {
            //        Email = $"email{e}@example.com",
            //        Flg = false,
            //        Name = "Name" + e
            //    });
            //});

            ////var database = new Repository<test>(new DatabaseService("Server=124.70.12.71;Port=3306;Database=jorenoe;Uid=root;Pwd=jorenoe123;"));
            //  var database = new Repository<test>(new DatabaseService("Server=43.136.101.66;Port=3306;Database=jorenoe;Uid=root;Pwd=jorenoe123;", IsEnabledMulitConnection: false, mulitInsertBatchcount: 200000));

            //var x = database.IsExists("Name544845", "Name");

            //var xx = database.Find(d => d.Name == "1" && d.Email == "1@");

            ////var database = new Repository<test>(new DatabaseService("Server=mysql.sqlpub.com;Port=3306;Database=mydbcloud;Uid=jorenoe;Pwd=48db25c68757687a;"));

            //for (int i = 0; i < 10; i++)
            //{
            //    Stopwatch stopwatch = new Stopwatch();
            //    Console.WriteLine("开始计时...");
            //    stopwatch.Start();

            //    //await database.TestMUlit().ConfigureAwait(false);
            //    database.BulkInsert(Data);

            //    stopwatch.Stop();
            //    Console.WriteLine("计时结束.");
            //    TimeSpan elapsedTime = stopwatch.Elapsed;
            //    Console.WriteLine($"总共耗时: {elapsedTime.TotalSeconds} 秒");
            //}

           // var services = new ServiceCollection();
           // services.AddHttpClientApi();

           // // 构建服务提供者
           // var serviceProvider = services.BuildServiceProvider();

           // // 获取 HttpClientApi 实例
           // var httpClientApi = serviceProvider.GetRequiredService<HttpClientApi>();

           //var ss = await  httpClientApi.GetAsync("https://jorenoe.top/dogegg/api/notice");


            var Service = new ServiceCollection();
            Service.AddJoreNoeDapper("Server=43.136.101.66;Port=3306;Database=jorenoe;Uid=root;Pwd=jorenoe123;", IDBType.MySql,true);
            Service.AddJoreNoeRedis("43.136.101.66:6379,password=JoreNoe123", 0);
            var serviceProvider = Service.BuildServiceProvider();
            var api = serviceProvider.GetRequiredService<JoreNoe.DB.Dapper.IRepository<test>>();

            //for (int i = 0; i < 100; i++)
            //{
            //    var result = api.Add(new test
            //    {
            //        Flg = true,
            //        Email = "123",
            //        Name = "test"
            //    });
            //}

            Parallel.For(0, 100, i =>
            {
                var result = api.AddAsync(new test
                {
                    Flg = true,
                    Email = "123",
                    Name = "test"
                });
            });

            Parallel.For(0, 100, i =>
            {
                var result = api.Add(new test
                {
                    Flg = true,
                    Email = "123",
                    Name = "test"
                });
            });



            // redis 并发测试

            var redisapi = serviceProvider.GetRequiredService<JoreNoe.Cache.Redis.IRedisManager>();
            var xxx = new ConcurrentBag<string>();
            Parallel.For(0, 10000, e =>
            {
                var ff = redisapi.Single("ProjectName_JoreNoe_Nuget_JoreNoe_Pack_Config");

                xxx.Add(ff);
            });

            Console.ReadLine();
        }
    }
}



// 消息队列使用 接受
//Register.RegisterQueue("124.70.12.71", "jorenoe", "jorenoe", "/", "hello");
//QueueManager.Receive(new PhoneStore(),"hello");
//Console.ReadLine();

//Register.RegisterQueue("124.70.12.71", "jorenoe", "jorenoe", "/", "hello"); 
//QueueManager.

// Redis缓存使用
//JoreNoeRedisBaseService RedisDataBase = new JoreNoeRedisBaseService(new SettingConfigs
//{
//    ConnectionString = "43.136.101.66:6379,password=jorenoe123",
//    DefaultDB = 1,
//    InstanceName = "TestRedis"
//});
//IRedisManager RedisManager = new RedisManager(RedisDataBase);
//RedisManager.Add("Test", "test", ExpireModel.LongCache);
//Console.WriteLine(RedisManager.Get("Test"));

// AutoMapper  封装
//var config = new MapperConfiguration(cfg =>
//{
//    cfg.CreateMap<test, test1>();
//    cfg.CreateMap<test1, test>();
//});
//var mapper = new Mapper(config);
//JoreNoe.Extend.JoreNoeObjectToObjectExtension.UseJoreNoeObjectToOBject(mapper);
//var test = new test() {
//    name = "c",
//    age=123
//};
//var test1 = new test1();

//// 将 test 数据 给 test1
//var ment = test.Map(test1);


// 邮箱
//JoreNoe.Message.IEmailMessageAPI MessageAPI = new JoreNoe.Message.EmailMessageAPI(new JoreNoe.Message.EmailMessageSettingConfigs { 
//    SmtpHost= "smtp.qiye.aliyun.com",
//    Password="zw.047600",
//    SmtpPort=25,
//    SmtpUserName= "postmaster@jorenoe.top"
//});

//MessageAPI.Send("jorenoe@126.com","测试接口","测试数据");

// Dapper 
// Registory.SetInitDbContext("Server=124.70.12.71;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql,500000);


// httpclient 使用方式
//var services = new ServiceCollection();
//services.AddHttpClientApi();

//// 构建服务提供者
//var serviceProvider = services.BuildServiceProvider();

//// 获取 HttpClientApi 实例
//var httpClientApi = serviceProvider.GetRequiredService<HttpClientApi>();

//var ss = await httpClientApi.GetAsync("https://jorenoe.top/dogegg/api/notice");

// 注入 services.AddHttpClientApi();