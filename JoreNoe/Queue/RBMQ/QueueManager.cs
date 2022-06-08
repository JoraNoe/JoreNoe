using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.Queue.RBMQ
{
    public class QueueManager
    {
        private static ConnectionFactory ConectionFactory;
        private static string QueueName = Register._ChannelName;
        public QueueManager()
        {
            ConectionFactory = Register.ConectionFactory;
        }


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
        public static void Receive<T>(ICustome Custome)
        {
            using (var connection = ConectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(QueueName, true, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());

                        Custome.ConSume<T>(new CustomeContent<T>
                        {
                            QueueName = QueueName,
                            Context = (T)JsonConvert.DeserializeObject(message)
                        });

                    };

                }
            }

        }
    }
}
