using RabbitMQ.Client;

namespace JoreNoe.Queue.RBMQ
{

    public enum MQSendEnum
    {
        Direct = 1,   //推送模式
        Fanout = 2,   //订阅模式
        Topic = 3     //主题路由模式
    }


    public class Register
    {

        /// <summary>
        /// 创建工厂
        /// </summary>
        public static ConnectionFactory ConectionFactory = null;

        public static string _ChannelName = "";

        /// <summary>
        /// 注册消息队列
        /// </summary>
        /// <param name="HostName"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="VirtualHost"></param>
        /// <param name="SendType"></param>
        public static void RegisterQueue(string HostName, string UserName, string Password, string VirtualHost, string ChannelName,int Port = 5672)
        {
            _ChannelName = ChannelName;

            ConectionFactory = new ConnectionFactory()
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password,
                Port = Port,
                VirtualHost = VirtualHost
            };
        }
    }
}
