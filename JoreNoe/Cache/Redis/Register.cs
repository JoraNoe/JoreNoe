﻿using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

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

        /// <summary>
        /// 是否启用项目名称获取作为主文件夹
        /// </summary>
        bool IsEnabledFaieldProjectName { get; set; }
    }

    /// <summary>
    /// 配置文件复制
    /// </summary>
    public class SettingConfigs : ISettingConfigs
    {
        public SettingConfigs()
        {

        }
        public SettingConfigs(string ConnectionString, int DefaultDB, bool IsEnabledFaieldProjectName)
        {
            this.ConnectionString = ConnectionString;
            this.DefaultDB = DefaultDB;
            this.IsEnabledFaieldProjectName = IsEnabledFaieldProjectName;
        }
        public string ConnectionString { get; set; }
        public int DefaultDB { get; set; }
        public bool IsEnabledFaieldProjectName { get; set; }
    }

    /// <summary>
    /// Redis基础接口
    /// </summary>
    public interface IJoreNoeRedisBaseService
    {
        IDatabase RedisDataBase { get; set; }

        IConnectionMultiplexer ConnectionMultiplexer { get; set; }

        ISettingConfigs SettingConfigs { get; set; }
    }

    /// <summary>
    /// 实现方法
    /// </summary>
    public class JoreNoeRedisBaseService : IJoreNoeRedisBaseService
    {
        public ISettingConfigs SettingConfigs { get; set; }
        public IDatabase RedisDataBase { get; set; }
        public IConnectionMultiplexer ConnectionMultiplexer { get; set; }

        public JoreNoeRedisBaseService(ISettingConfigs SettingConfigs, IConnectionMultiplexer connectionMultiplexer)
        {
            this.SettingConfigs = SettingConfigs;
            this.ConnectionMultiplexer = connectionMultiplexer;
            this.RedisDataBase = connectionMultiplexer.GetDatabase(this.SettingConfigs.DefaultDB);
        }
    }

    public class JoreNoeRedisEntry
    {
        public string ConnectionString { get; set; }
        public int DefaultDB { get; set; }
        public bool IsEnabledFaieldProjectName { get; set; }
        public int DefaultRedisExpire { get; set; }
    }

    /// <summary>
    /// 实用类
    /// </summary>
    public static class JoreNoeRedisExtensions
    {
        /// <summary>
        /// 注册Redis 服务
        /// 包括中间件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="DefaultDB"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddJoreNoeRedis(this IServiceCollection services, Action<JoreNoeRedisEntry> Expires)
        {
            var ExpiresInvoke = new JoreNoeRedisEntry();
            Expires(ExpiresInvoke);
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var lazyConnection = new Lazy<IConnectionMultiplexer>(() =>
                {
                    try
                    {
                        return ConnectionMultiplexer.Connect(ExpiresInvoke.ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        // 处理连接异常
                        throw new InvalidOperationException("Unable to connect to Redis", ex);
                    }
                });

                // 返回 Lazy 对象，懒加载时会触发连接初始化
                return lazyConnection.Value;
            });


            services.AddSingleton<ISettingConfigs>(new SettingConfigs(ExpiresInvoke.ConnectionString, ExpiresInvoke.DefaultDB, ExpiresInvoke.IsEnabledFaieldProjectName));
            services.AddSingleton<IJoreNoeRedisBaseService, JoreNoeRedisBaseService>();
            services.AddSingleton(typeof(IRedisManager), typeof(RedisManager));
            //services.AddScoped(typeof(IRedisManager), typeof(RedisManager));
        }

        /// <summary>
        /// 使用Redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="DefaultDB"></param>
        /// <param name="IsEnabledFaieldProjectName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddJoreNoeRedis(this IServiceCollection services, string ConnectionString, int DefaultDB, bool IsEnabledFaieldProjectName = false)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var lazyConnection = new Lazy<IConnectionMultiplexer>(() =>
                {
                    try
                    {
                        return ConnectionMultiplexer.Connect(ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        // 处理连接异常
                        throw new InvalidOperationException("Unable to connect to Redis", ex);
                    }
                });

                // 返回 Lazy 对象，懒加载时会触发连接初始化
                return lazyConnection.Value;
            });


            services.AddSingleton<ISettingConfigs>(new SettingConfigs(ConnectionString, DefaultDB, IsEnabledFaieldProjectName));
            services.AddSingleton<IJoreNoeRedisBaseService, JoreNoeRedisBaseService>();
            services.AddScoped(typeof(IRedisManager), typeof(RedisManager));
            //services.AddScoped(typeof(IRedisManager), typeof(RedisManager));
        }
    }
}
