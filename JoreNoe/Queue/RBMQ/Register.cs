using JoreNoe.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HSSF.UserModel;
using RabbitMQ.Client;
using System;

namespace JoreNoe.Queue.RBMQ
{

    public enum MQSendEnum
    {
        Direct = 1,   //推送模式
        Fanout = 2,   //订阅模式
        Topic = 3     //主题路由模式
    }

    public interface IJoreNoeRabbitMQSettingConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }


        public int Port { get; set; }
    }

    public class JoreNoeRabbitMQSettingConfiguration : IJoreNoeRabbitMQSettingConfiguration
    {
        public JoreNoeRabbitMQSettingConfiguration(string HostName, string UserName, string Password, string VirtualHost, int Port = 5672)
        {
            this.HostName = HostName;
            this.UserName = UserName;
            this.Password = Password;
            this.VirtualHost = VirtualHost;
            this.Port = Port;
        }

        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
    }

    public interface IJoreNoeRabbitMQBaseService
    {
        public IConnectionFactory ConnectionFactory { get; set; }

        public IJoreNoeRabbitMQSettingConfiguration Configuration { get; set; }
    }

    public class JoreNoeRabbitMQBaseService : IJoreNoeRabbitMQBaseService
    {
        public JoreNoeRabbitMQBaseService(IJoreNoeRabbitMQSettingConfiguration Configuration, IConnectionFactory Connection)
        {
            this.Configuration = Configuration;
            this.ConnectionFactory = Connection;
        }

        public IConnectionFactory ConnectionFactory { get; set; }
        public IJoreNoeRabbitMQSettingConfiguration Configuration { get; set; }
    }

    public static class JoreNoeRabbitMQExtensions
    {
        public static void AddJoreNoeRabbitMQ(this IServiceCollection Services, string ConnectionString)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new Exception(ConnectionString);

            var uriBuilder = new UriBuilder(ConnectionString);

            // 设置连接工厂
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(uriBuilder.ToString().Split('?')[0]); // 移除查询参数

            // 创建配置对象
            var configuration = new JoreNoeRabbitMQSettingConfiguration(
                HostName: connectionFactory.HostName,
                UserName: connectionFactory.UserName,
                Password: connectionFactory.Password,
                VirtualHost: connectionFactory.VirtualHost,
                Port: connectionFactory.Port
            );

            Services.AddSingleton<IJoreNoeRabbitMQSettingConfiguration>(configuration);
            Services.AddSingleton<IJoreNoeRabbitMQBaseService,JoreNoeRabbitMQBaseService>();
            Services.AddSingleton<IConnectionFactory>(connectionFactory);
            Services.AddSingleton<IQueueManger, QueueManager>();
        }
    }
}
