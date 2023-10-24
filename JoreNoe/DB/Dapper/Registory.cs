using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace JoreNoe.DB.Dapper
{
    public enum IDBType { SqlServer = 0, MySql = 1 };

    public class Registory
    {
        /// <summary>
        /// 链接
        /// </summary>
        public static IDbConnection _Connection { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public static IDBType ConnectionDbType { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private static IDbConnection CreateDbConnection(string connectionString, IDBType dbType)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            switch (dbType)
            {
                case IDBType.MySql:
                    return new MySqlConnection(connectionString);
                case IDBType.SqlServer:
                    return new SqlConnection(connectionString);
                default:
                    throw new Exception("未知类型");
            }
        }

        /// <summary>
        /// 初始化Dapper链接
        /// </summary>
        /// <param name="DBConnectionString"></param>
        /// <param name="DBType"></param>
        public static void SetInitDbContext(string DBConnectionString, IDBType DBType)
        {
            ConnectionDbType = DBType;
            ConnectionString = DBConnectionString;
            _Connection = CreateDbConnection(DBConnectionString, DBType);
        }

        /// <summary>
        /// 服务注入
        /// </summary>
        /// <param name="Services"></param>
        public static void AddJoreNoeDpper(IServiceCollection Services)
        {
            _ = Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
