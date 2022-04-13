using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace JoreNoe.Cache.Redis
{
    public class Register
    {
        //连接字符串
        private static string _connectionString;
        //实例名称
        private static string _instanceName;
        //默认数据库
        private static int _defaultDB;

        protected static ConcurrentDictionary<string, ConnectionMultiplexer> _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public static void SetInitRedisConfig(string ConnectionString, string InstanceName, int DefaultDB = 0)
        {
            _connectionString = ConnectionString;
            _instanceName = InstanceName;
            _defaultDB = DefaultDB;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        protected static IDatabase GetDatabase()
        {
            return GetConnect().GetDatabase(_defaultDB);
        }
        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetConnect()
        {
            return _connections.GetOrAdd(_instanceName, p => ConnectionMultiplexer.Connect(_connectionString));
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
