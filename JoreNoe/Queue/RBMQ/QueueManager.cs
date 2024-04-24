using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Queue.RBMQ
{
    public class QueueManager
    {
        private static ConnectionFactory ConectionFactory = Register.ConectionFactory;
        private static string QueueName = Register._ChannelName;

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="QueueChannle"></param>
        public static void SendPublish<T>(T Entity, string Type = ExchangeType.Topic)
        {
            using (var connection = ConectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(QueueName, Type);
                    channel.QueueDeclare(QueueName, false, false, false, null);
                    string message = JsonConvert.SerializeObject(Entity);
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", QueueName, properties, body);
                }
            }
        }


        /// <summary>
        /// 接收消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Custome"></param>
        public static void Receive<T>(ICustome<T> Custome, string QueueName) where T : class
        {
            Task.Run(async () =>
            {
                var connection = ConectionFactory.CreateConnection();
                var channel = connection.CreateModel();
                while (true)
                {
                    channel.QueueDeclare(queue: QueueName,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
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

                            throw new Exception(ex.Message);
                        }

                    };
                    channel.BasicConsume(queue: QueueName,
                                         autoAck: true,
                                         consumer: consumer);
                    await Task.Delay(1000);
                }


            }).ConfigureAwait(false);


        }

        static bool IsValidJson(string json)
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
