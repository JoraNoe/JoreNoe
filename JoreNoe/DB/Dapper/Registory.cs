using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace JoreNoe.DB.Dapper
{
    /// <summary>
    /// 数据库枚举
    /// </summary>
    public enum IDBType { SqlServer, MySql };

    /// <summary>
    /// 数据库配置参数
    /// </summary>
    public interface IDatabaseSettings
    {
        IDBType dbType { get; set; }
        string connectionString { get; set; }
        long mulitInsertBatchcount { get; set; }
        bool IsEnabledMulitConnection { get; set; }
    }

    /// <summary>
    /// 数据库使用服务
    /// </summary>
    public interface IDatabaseService
    {
        IDatabaseSettings DataBaseSettings { get; set; }
        IDbConnection GetConnection();
    }


    public class DatabaseSettings : IDatabaseSettings
    {
        public DatabaseSettings(string connectionString, IDBType dbtype = IDBType.MySql, bool IsEnabledMulitConnection = false, long mulitInsertBatchcount = 200000)
        {
            this.connectionString = connectionString;
            this.dbType = dbtype;
            this.mulitInsertBatchcount = mulitInsertBatchcount;
            this.IsEnabledMulitConnection = IsEnabledMulitConnection;
        }
        public IDBType dbType { set; get; }
        public string connectionString { set; get; }
        public long mulitInsertBatchcount { set; get; }
        public bool IsEnabledMulitConnection { get; set; }
    }


    public static class JoreNoeDapperExtensions
    {
        public static void AddJoreNoeDapper(this IServiceCollection services, string connectionString, IDBType dbtype, bool IsEnabledMulitConnection = false, long mulitInsertBatchcount = 200000)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IDatabaseSettings>(new DatabaseSettings(connectionString, dbtype, IsEnabledMulitConnection, mulitInsertBatchcount));
            services.AddScoped<IDatabaseService, DatabaseService>();

        }
    }

    public class DatabaseService : IDatabaseService
    {
        public DatabaseService(IDatabaseSettings dataBaseSettings)
        {
            this.DataBaseSettings = dataBaseSettings;
        }

        public DatabaseService(string connectionString, IDBType dbtype = IDBType.MySql,bool IsEnabledMulitConnection = false, long mulitInsertBatchcount = 200000)
        {
            this.DataBaseSettings = new DatabaseSettings(connectionString, dbtype, IsEnabledMulitConnection, mulitInsertBatchcount);
        }

        public IDatabaseSettings DataBaseSettings { get; set; }

        public IDbConnection GetConnection()
        {
            if (this.DataBaseSettings == null)
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(this.DataBaseSettings));
            if (string.IsNullOrEmpty(this.DataBaseSettings.connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(this.DataBaseSettings.connectionString));

            switch (this.DataBaseSettings.dbType)
            {
                case IDBType.MySql:
                    return new MySqlConnection(this.DataBaseSettings.connectionString);
                case IDBType.SqlServer:
                    return new Microsoft.Data.SqlClient.SqlConnection(this.DataBaseSettings.connectionString);
                default:
                    throw new NotSupportedException($"Database type '{this.DataBaseSettings.dbType}' is not supported.");
            }

        }
    }

}
