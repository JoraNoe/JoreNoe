using JoreNoe.DB.Dapper;
using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;
using System;

namespace ConsoleApp6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Registory.SetInitDbContext("Server=124.70.12.71;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql);
            var database = new Repository<test>();
            database.Add(new test { Name = "123", Email = "123@qq.com" });

            database.Update(1, e => {
                e.Name = "修改后的";
                e.Flg = true;
            });

            database.Update(1, e=>e.Name = "123");

            database.Update(2, new test
            {
                Name = "修改后的2"
            });


            //var x = database.IsExists(12000340, "employee_id");

        }

        public class test
        {
            [IgnoreAutoIncrement]
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
