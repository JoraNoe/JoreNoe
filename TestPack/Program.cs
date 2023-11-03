
using Dapper.Contrib.Extensions;
using JoreNoe.DB.Dapper;
using NPOI.Util;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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


            await TestInsert();

            //await TestInsert();
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
                    x.Enqueue(item+"");
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
                list.Add(i+string.Empty);
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

            Registory.SetInitDbContext("Server=172.26.127.108;Database=JoreNoe;User ID=root;Password=123456;", IDBType.MySql);

            var database = new Repository<Employees>();
            var lists = new ConcurrentQueue <Employees>();
            for (int i = 0; i < 10000000; i++)
            {
                lists.Enqueue(new Employees
                {
                    employee_id = i,
                    first_name = "asdf",
                    last_name = "123456",
                    hire_date = DateTime.Now,
                    email = "12@qq.com",
                    ceshi = true,
                    id_number = "123",
                    phone_number = "12341234",
                    verification_code = "12341"
                });
                Console.WriteLine(i);
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
        public int employee_id { get; set; }
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
