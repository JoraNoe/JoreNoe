using JoreNoe.DB.Dapper;
using Microsoft.Win32;
using System.Data;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Registory.SetInitDbContext("Server=mysql.sqlpub.com;Database=mydbcloud;User Id=jorenoe;Password=48db25c68757687a;", IDBType.MySql);
            var data = new Repository<Employees>();

            //批量插入
            var lists = new List<Employees>();
            for (int i = 0; i < 400; i++)
            {
                lists.Add(new Employees { employee_id = 1000 + i, first_name = "asdf", last_name = "123456" });
            }

            data.BulkInsert(lists);

        }
    }
    public class Employees
    {
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}