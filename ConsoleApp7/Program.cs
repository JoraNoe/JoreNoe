using JoreNoe.DB.Dapper;
using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;

namespace ConsoleApp7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var database = new Repository<test>(new DatabaseService("Server=124.70.12.71;Database=jorenoe;User ID=root;Password=jorenoe123;"));


            // Registory.SetInitDbContext("Server=124.70.12.71;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql,500000);
            //Registory.SetInitDbContext("Server=43.136.101.66;Database=jorenoe;User ID=root;Password=jorenoe123;", IDBType.MySql);

            var ment = new List<test>();
            for (int i = 0; i < 10000; i++)
            {
                ment.Add(new test
                {
                    Flg = true,
                    Email = "123@" + i,
                    Name = "asdasdf"
                });
            }


            database.BulkInsert(ment);



            Console.WriteLine("完成");
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
