using JoreNoe.DB.Dapper;
using System;
using System.Collections.Generic;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Registory.SetInitDbContext("Server=mysql.sqlpub.com;Database=mydbcloud;User Id=jorenoe;Password=48db25c68757687a;", IDBType.MySql);


            var database = new Repository<Employees>();
            var lists = new List<Employees>();
            for (int i = 0; i < 400; i++)
            {
                lists.Add(new Employees { employee_id = 1234 + i, first_name = "asdf", last_name = "123456" });
            }

            database.BulkInsert(lists);

        }
    }
    public class Employees
    {
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}
