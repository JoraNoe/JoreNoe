
using Dapper.Contrib.Extensions;
using JoreNoe.DB.Dapper;
using NPOI.Util;
using System;
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
            //Registory.SetInitDbContext("Server=mysql.sqlpub.com;Database=mydbcloud;User Id=jorenoe;Password=48db25c68757687a;", IDBType.MySql);

            Registory.SetInitDbContext("Server=119.3.208.175;Database=ouconline_allinone_datacenter;User ID=ouconline_allinone_datacenter;Password=zeNnwfyD5ue2z81V;", IDBType.MySql);

            var database = new Repository<Employees>();
            var lists = new List<Employees>();
            for (int i = 0; i < 1000000; i++)
            {
                lists.Add(new Employees { employee_id = 1234 + i, first_name = "asdf", last_name = "123456",hire_date=DateTime.Now,email="12@qq.com",ceshi=true });
            }

            for (int i = 0; i < 10; i++)
            {
                database.Removes<int>(lists.Select(d => d.employee_id).ToArray(), "employee_id");
                // 创建 Stopwatch 对象
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                await database.BulkInsertAsync(lists);
                // 停止计时
                stopwatch1.Stop();
                // 获取经过的时间
                TimeSpan elapsed1 = stopwatch1.Elapsed;

                // 输出经过的时间
                Console.WriteLine($"封装2经过的时间: {elapsed1}");
            }

            

        }
    }
    public class Employees
    {
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        public string email { get; set; }

        public DateTime hire_date { get; set; }

        public bool ceshi { get; set; }
    }
}
