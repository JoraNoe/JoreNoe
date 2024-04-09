using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using JoreNoe.Queue.RBMQ;

namespace ConsoleApp1
{

    public class test {
        public string name { get; set; }
        public int age { get; set; }
    }

    public class PhoneStore:ICustome<test>
    {
        public async Task<test> ConSume(CustomeContent<test> Context)
        {
            await Console.Out.WriteLineAsync(Context.Context.name);
            return null;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Register.RegisterQueue("124.70.12.71", "jorenoe", "jorenoe", "/", "hello");
            QueueManager.Receive(new PhoneStore(),"hello");
            Console.ReadLine();
        }
    }

}
