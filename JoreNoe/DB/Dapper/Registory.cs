using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

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
    }
}
