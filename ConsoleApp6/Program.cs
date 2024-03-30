using JoreNoe.DB.Dapper;
using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleApp6
{
    internal class Program
    {

        static void Main(string[] args)
        {


            Registory.SetInitDbContext("Server=124.70.12.71;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql);
            //Registory.SetInitDbContext("Server=43.136.101.66;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql);
            var database = new Repository<test>();


            for (int i = 0; i < 100000; i++)
            {
                database.PublishHistory("我是测试"+i, "测试"+i, "类型"+i);
                Console.WriteLine("结束"+DateTime.Now);
            }


            // 推送日志
            database.PublishHistory("我是测试","测试","类型");

            database.Update(1,e=>new test{ Email = "6",Flg = false,Name = "6"});

            database.Update(1, e => {
                e.Name = "日币日币";
                e.Flg = false;
            });


            database.Update(1,new test {
                Name = "132132412412341123",
            });


            database.Add(new test { Name = "123", Email = "123@qq.com" });

            database.Update(1, e => {
                e.Name = "修改后的";
                e.Flg = true;
            });

            database.Update(1, e=>e.Name = "123");

            var x = new test { Name = "123", Flg = false };
            database.Update(1, new { x });


           

            //var x = database.IsExists(12000340, "employee_id");

        }

        public class test
        {
            [InsertIgnoreAutoIncrementAttribute]
            public int Id { get; set; }

            public string? Name { get; set; }

            /// <summary>
            /// 邮箱
            /// </summary>
            public string? Email { get; set; }

            public bool Flg { get; set; }
        }
    }
}
