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


            Registory.SetInitDbContext("Server=124.70.12.71;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql,500000);
            //Registory.SetInitDbContext("Server=43.136.101.66;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql);
            var database = new Repository<test>();


            //for (int i = 0; i < 100000; i++)
            //{
            //    database.PublishHistory("我是测试"+i, "测试"+i, "类型"+i);
            //    Console.WriteLine("结束"+DateTime.Now + "-" + i);
            //}

            var ment = new List<test>();
            for (int i = 0;i < 1000000; i++)
            {
                ment.Add(new test { 
                    Flg = true,
                    Email = "123@"+i,
                    Name = "asdasdf"
                });
            }

            
            database.BulkInsert(ment);

            
            Console.WriteLine("完成");


            //var x = database.IsExists(12000340, "employee_id");

        }

        public class test
        {
            [InsertIgnoreAutoIncrementAttribute]
            public int Id { get; set; }

            public string Name { get; set; }

            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get; set; }

            public bool Flg { get; set; }
        }
    }
}
