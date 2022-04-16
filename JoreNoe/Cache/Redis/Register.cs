using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace JoreNoe.Cache.Redis
{

    public static class Register
    {
        /// <summary>
        /// 链接字符串
        /// </summary>
        private static string _ConnectionString { get; set; }

        /// <summary>
        /// 实例名称
        /// </summary>
        private static string _InstanceName { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        private static int _DefaultDB { get; set; }

        public static ConcurrentDictionary<string, ConnectionMultiplexer> _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public static void InitRedisConfig(string ConnectionString, string InstanceName, int DefaultDB = 0)
        {
            _ConnectionString = ConnectionString;
            _InstanceName = InstanceName;
            _DefaultDB = DefaultDB;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public static IDatabase GetDatabase()
        {
            return GetConnect().GetDatabase(_DefaultDB);
        }
        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetConnect()
        {
            return _connections.GetOrAdd(_InstanceName, p => ConnectionMultiplexer.Connect(_DefaultDB.ToString()));
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
