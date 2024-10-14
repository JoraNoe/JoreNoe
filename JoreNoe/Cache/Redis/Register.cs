using JoreNoe.DB.Dapper;
using JoreNoe.Limit;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace JoreNoe.Cache.Redis
{

    public interface ISettingConfigs
    {
        /// <summary>
        /// 字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        int DefaultDB { get; set; }
    }

    /// <summary>
    /// 配置文件复制
    /// </summary>
    public class SettingConfigs : ISettingConfigs
    {
        public SettingConfigs()
        {

        }
        public SettingConfigs(string ConnectionString, int DefaultDB = 0)
        {
            this.ConnectionString = ConnectionString;
            this.DefaultDB = DefaultDB;
        }

        public string ConnectionString { get; set; }
        public int DefaultDB { get; set; }
    }

    /// <summary>
    /// Redis基础接口
    /// </summary>
    public interface IJoreNoeRedisBaseService
    {
        Lazy<ConnectionMultiplexer> ConnectionDB { get; set; }

        IDatabase RedisDataBase { get; set; }
    }

    /// <summary>
    /// 实现方法
    /// </summary>
    public class JoreNoeRedisBaseService : IJoreNoeRedisBaseService
    {
        private readonly ISettingConfigs SettingConfigs;
        public Lazy<ConnectionMultiplexer> ConnectionDB { get; set; }
        public IDatabase RedisDataBase { get; set; }

        public JoreNoeRedisBaseService(ISettingConfigs SettingConfigs)
        {
            this.SettingConfigs = SettingConfigs;
            this.ConnectionDB = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(this.SettingConfigs.ConnectionString));
            this.RedisDataBase = this.ConnectionDB.Value.GetDatabase(this.SettingConfigs.DefaultDB);
        }
    }

    /// <summary>
    /// 实用类
    /// </summary>
    public static class JoreNoeRedisExtensions
    {
        public static void AddJoreNoeRedis(this IServiceCollection services, string ConnectionString, int DefaultDB = 0)
        {
            services.AddSingleton<ISettingConfigs>(new SettingConfigs(ConnectionString, DefaultDB));
            services.AddScoped<IJoreNoeRedisBaseService, JoreNoeRedisBaseService>();
            services.AddScoped(typeof(IRedisManager), typeof(RedisManager));
        }
    }
}
