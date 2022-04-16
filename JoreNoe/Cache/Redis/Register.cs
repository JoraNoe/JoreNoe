using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace JoreNoe.Cache.Redis
{
    public class RedisEntity
    {
        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DefaultDB { get; set; }
    }

    public static class Register
    {
        public static RedisEntity RedisEntity;
        public static ConcurrentDictionary<string, ConnectionMultiplexer> _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public static void InitRedisConfig(RedisEntity Entity)
        {
            RedisEntity = Entity;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public static IDatabase GetDatabase()
        {
            return GetConnect().GetDatabase(RedisEntity.DefaultDB);
        }
        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetConnect()
        {
            return _connections.GetOrAdd(RedisEntity.InstanceName, p => ConnectionMultiplexer.Connect(RedisEntity.DefaultDB.ToString()));
        }


        /// <summary>
        /// 服务
        /// </summary>
        /// <param name="Services"></param>
        public static void AddJoreNoeRedis(IServiceCollection Services)
        {
            _ = Services.AddSingleton<IRedisManager, RedisManager>();
        }


    }
}
