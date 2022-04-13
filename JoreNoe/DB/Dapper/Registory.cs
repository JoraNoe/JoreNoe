using JoreNoe.DB.EntityFrameWork.Core.SqlServer;
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

        public static IDBType ConnectionDbType { get; set; }

        /// <summary>
        /// 设置上下文方法
        /// </summary>
        /// <param name="DB"></param>
        public static void SetInitDbContext(string DBConnectionString, IDBType DBType)
        {
            if (string.IsNullOrEmpty(DBConnectionString))
                throw new ArgumentNullException(nameof(DBConnectionString));

            ConnectionDbType = DBType;

            if (DBType == IDBType.MySql)
            {
                _Connection = new MySqlConnection(DBConnectionString);
            }
            else if (DBType == IDBType.SqlServer)
            {
                _Connection = new SqlConnection(DBConnectionString);
            }
            else
            {
                throw new Exception("未知类型");
            }
        }

        /// <summary>
        /// 服务注入
        /// </summary>
        /// <param name="Services"></param>
        public static void AddJoreNoeDpper(IServiceCollection Services)
        {
            _ = Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}
