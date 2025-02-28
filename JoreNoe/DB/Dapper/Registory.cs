using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JoreNoe.DB.Dapper
{
    /// <summary>
    /// 数据库枚举
    /// </summary>
    public enum IDBType { SqlServer, MySql };

    /// <summary>
    /// 数据库配置参数
    /// </summary>
    //public interface IDatabaseSettings
    //{
    //    IDBType dbType { get; set; }
    //    string connectionString { get; set; }
    //    long mulitInsertBatchcount { get; set; }
    //    bool IsEnabledMulitConnection { get; set; }
    //    string MulitDBConnectionName { get; set; }
    //}
    public class DatabaseSettings
    {
        public DatabaseSettings(string connectionString, IDBType dbtype = IDBType.MySql, bool IsEnabledMulitConnection = false, long mulitInsertBatchcount = 200000, IList<string> AvailableTables = null)
        {
            this.connectionString = connectionString;
            this.dbType = dbtype;
            this.mulitInsertBatchcount = mulitInsertBatchcount;
            this.IsEnabledMulitConnection = IsEnabledMulitConnection;
            this.MulitDBConnectionName = DapperExtend.GetDatabaseName(connectionString);
            this.AvailableTables = AvailableTables;

        }
        public IDBType dbType { set; get; }
        public string connectionString { set; get; }
        public long mulitInsertBatchcount { set; get; }
        public bool IsEnabledMulitConnection { get; set; }
        public string MulitDBConnectionName { get; set; }
        public IList<string> AvailableTables { get; set; }
    }

    public interface IMulitDatabaseSettings
    {
        bool IsMulit { get; set; }
        IList<DatabaseSettings> DatabaseSettings { get; set; }
    }

    public class MulitDatabaseSettings : IMulitDatabaseSettings
    {
        public MulitDatabaseSettings(List<DatabaseSettings> DatabaseSettings, bool IsMulit)
        {
            this.DatabaseSettings = DatabaseSettings;
            this.IsMulit = IsMulit;
        }

        public IList<DatabaseSettings> DatabaseSettings { get; set; }
        public bool IsMulit { get; set; }
    }




    public static class JoreNoeDapperExtensions
    {
        /// <summary>
        /// 注册单个链接字符串上下文Dapper封装方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">单个链接字符串</param>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="IsEnabledMulitConnection">是否启用并发模式，支持创建多个上下文</param>
        /// <param name="mulitInsertBatchcount">批量插入，分批一批数量</param>
        public static void AddJoreNoeDapper(this IServiceCollection services, string connectionString, IDBType dbtype, bool IsEnabledMulitConnection = false, long mulitInsertBatchcount = 200000)
        {
            var AssamblyClassSettings = new DatabaseSettings(connectionString, dbtype, IsEnabledMulitConnection, mulitInsertBatchcount);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IMulitDatabaseSettings>(new MulitDatabaseSettings(new List<DatabaseSettings> { AssamblyClassSettings }, false));
            services.AddScoped<IDatabaseService, DatabaseService>();
        }


        public static void AddJoreNoeDapper(this IServiceCollection services, List<DatabaseSettings> Params)
        {
            var GetDataBaseConnectionSettings = Params;
            if (Params == null || Params.Any(d => d.AvailableTables == null || d.AvailableTables.Count == 0))
            {
                throw new NoNullAllowedException("多数据库，请填写表名,以确保程序可以准确找到那个表对应那个数据库");
            }
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IMulitDatabaseSettings>(new MulitDatabaseSettings(GetDataBaseConnectionSettings, true));
            services.AddScoped<IDatabaseService, DatabaseService>();
        }
    }

    /// <summary>
    /// 数据库使用服务
    /// </summary>
    public interface IDatabaseService
    {
        //IDatabaseSettings DataBaseSettings { get; set; }
        IDbConnection GetConnection(string MulitDBConnectionName);
        IMulitDatabaseSettings MulitDatabaseSettings { get; set; }

        Dictionary<string, DatabaseSettings> DataBaseSettingsLookUp { get; set; }
    }

    public class DatabaseService : IDatabaseService
    {
        /// <summary>
        /// 解析为字典 提升性能 
        /// </summary>
        public Dictionary<string, DatabaseSettings> DataBaseSettingsLookUp { get; set; }

        public DatabaseService(IMulitDatabaseSettings MulitDatabaseSettings)
        {
            this.MulitDatabaseSettings = MulitDatabaseSettings;

            if (this.MulitDatabaseSettings.IsMulit)
                DataBaseSettingsLookUp = MulitDatabaseSettings.DatabaseSettings
                .SelectMany(d => d.AvailableTables.Select(tableName => new { tableName, d }))
                .ToDictionary(x => x.tableName.ToLower(), x => x.d);
            else
                DataBaseSettingsLookUp = new Dictionary<string, DatabaseSettings> {
                    { "Default", this.MulitDatabaseSettings.DatabaseSettings[0] }
                };
        }

        public DatabaseService(MulitDatabaseSettings MulitDatabaseSettings)
        {
            this.MulitDatabaseSettings = MulitDatabaseSettings;

            if (this.MulitDatabaseSettings.IsMulit)
                DataBaseSettingsLookUp = MulitDatabaseSettings.DatabaseSettings
                .SelectMany(d => d.AvailableTables.Select(tableName => new { tableName, d }))
                .ToDictionary(x => x.tableName.ToLower(), x => x.d);
            else
                DataBaseSettingsLookUp = new Dictionary<string, DatabaseSettings> {
                    { "Default", this.MulitDatabaseSettings.DatabaseSettings[0] }
                };
        }

        public IMulitDatabaseSettings MulitDatabaseSettings { get; set; }

        /// <summary>
        /// 分发并且返回创建好的链接
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IDbConnection GetConnection(string MulitDBConnectionName)
        {
            if (!this.MulitDatabaseSettings.IsMulit)
            {
                var Get = DataBaseSettingsLookUp.TryGetValue("Default", out var SingleMulitSettings);
                if (!this.MulitDatabaseSettings.IsMulit && !Get)
                    throw new ArgumentException("No database settings found for default.", "Default");
                else
                    return this.CreateSingleConnection(SingleMulitSettings.connectionString, SingleMulitSettings.dbType);
            }
            else
            {
                var Mulit = DataBaseSettingsLookUp.TryGetValue(MulitDBConnectionName.ToLower(), out var SingleSettings);
                if (this.MulitDatabaseSettings.IsMulit && !Mulit)
                    throw new ArgumentException($"No database settings found for {MulitDBConnectionName}.", nameof(MulitDBConnectionName));
                else
                {
                    if (SingleSettings == null)
                        throw new ArgumentException("Connection string cannot be null or empty.", nameof(SingleSettings));
                    if (string.IsNullOrEmpty(SingleSettings.connectionString))
                        throw new ArgumentException("Connection string cannot be null or empty.", nameof(SingleSettings.connectionString));

                    return this.CreateSingleConnection(SingleSettings.connectionString, SingleSettings.dbType);
                }
            }
        }


        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="DBConnectionString"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public IDbConnection CreateSingleConnection(string DBConnectionString, IDBType Type)
        {
            switch (Type)
            {
                case IDBType.MySql:
                    return new MySqlConnection(DBConnectionString);
                case IDBType.SqlServer:
                    return new Microsoft.Data.SqlClient.SqlConnection(DBConnectionString);
                default:
                    throw new NotSupportedException($"Database type '{Type}' is not supported.");
            }
        }
    }

}
