using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace JoreNoe.Queue.RBMQ
{
    public interface IQueueManger
    {
        void SendPublish<T>(T Entity, string QueueName, string Type = ExchangeType.Topic);
        void Receive<T>(ICustome<T> Custome, string QueueName) where T : class;
    }
    public class QueueManager : IQueueManger
    {
        private readonly IJoreNoeRabbitMQBaseService JoreNoeRabbitMQBaseService;
        private IConnection connection;
        private IModel channel;

        public QueueManager(IJoreNoeRabbitMQBaseService JoreNoeRabbitMQBaseService)
        {
            this.JoreNoeRabbitMQBaseService = JoreNoeRabbitMQBaseService;
            this.connection = this.JoreNoeRabbitMQBaseService.ConnectionFactory.CreateConnection();
            this.channel = connection.CreateModel();
        }

        public void SendPublish<T>(T Entity, string QueueName, string Type = ExchangeType.Topic)
        {
            channel.ExchangeDeclare(QueueName, Type);
            channel.QueueDeclare(QueueName, true, false, false, null); // durable = true
            string message = JsonConvert.SerializeObject(Entity);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // 设置消息为持久化

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", QueueName, properties, body);
        }

        public void Receive<T>(ICustome<T> Custome, string QueueName) where T : class
        {
            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(0, 1, false); // 控制消息的预取数量，批量处理消息

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (IsValidJson(message))
                    {
                        Custome.ConSume(new CustomeContent<T>
                        {
                            QueueName = QueueName,
                            Context = JsonConvert.DeserializeObject<T>(message)
                        });
                    }
                    else
                    {
                        throw new Exception("无法序列化");
                    }
                }
                catch (Exception ex)
                {
                    // Log error (需要添加日志系统)
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };
            channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer); // 手动确认
        }

        private static bool IsValidJson(string json)
        {
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }

}
