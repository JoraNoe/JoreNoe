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

namespace ConsoleApp1
{

    public class test
    {
        public string name { get; set; }
        public int age { get; set; }
    }
    public class test1
    {
        public string name { get; set; }
        public int age { get; set; }

        public DateTime time { get; set; }

        public string OrganizationId { get; set; }
    }

    public class PhoneStore : ICustome<test>
    {
        public async Task<test> ConSume(CustomeContent<test> Context)
        {
            await Console.Out.WriteLineAsync(Context.Context.name);
            return null;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {

            var test = new List<test1>();

            test.Add(new test1
            {
                age = 1,
                name = "1" ,
                time = DateTime.Parse("2024-04-09 9:30"),
                OrganizationId = "802"
            });

            test.Add(new test1
            {
                age = 1,
                name = "2",
                time = DateTime.Parse("2024-04-19 9:13"),
                OrganizationId = "809"
            });

            test.Add(new test1
            {
                age = 1,
                name = "3",
                time = DateTime.Parse("2024-04-19 10:19"),
                OrganizationId = "802"
            });

            test.Add(new test1
            {
                age = 1,
                name = "2",
                time = DateTime.Parse("2024-04-18 11:31"),
                OrganizationId = "802"
            });

            var me = test.Where(d => new string[] { "1", "2" }.Contains(d.name)).ToList();

            var ces = test.Where(d => d.name == "1").ToList();

            var sf = new string[] { "809" };


            var FilterFormatTimeTenMin1 = DateTime.Parse(DateTime.Now.AddMinutes(24*60).ToString("yyyy-MM-dd HH:mm"));
            var EffectiveTenMinData1 = test.Where(d =>
                    DateTime.Parse(d.time.ToString("yyyy-MM-dd HH:mm")) >= FilterFormatTimeTenMin1
                    && DateTime.Parse(d.time.ToString("yyyy-MM-dd HH:mm")) <= FilterFormatTimeTenMin1
                ).ToList();

            var currentTimeMinusOneHour = DateTime.Parse(DateTime.Now.AddMinutes(60).ToString("yyyy-MM-dd HH:mm"));
            var effectiveData = test.Where(d =>
                DateTime.Parse(d.time.ToString("yyyy-MM-dd HH:mm")) >= currentTimeMinusOneHour.AddMinutes(-1)
                    && DateTime.Parse(d.time.ToString("yyyy-MM-dd HH:mm")) <= currentTimeMinusOneHour.AddMinutes(1) &&
                !sf.Contains(d.OrganizationId)) // 不包含在指定的组织列表中
                .ToList();


            Console.ReadLine();
        }
    }

}



// 消息队列使用
//Register.RegisterQueue("124.70.12.71", "jorenoe", "jorenoe", "/", "hello");
//QueueManager.Receive(new PhoneStore(),"hello");
//Console.ReadLine();

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