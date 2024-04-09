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

    
    public class PhoneStore:ICustome<string>
    {
        public PhoneStore()
        {
            
        }

        public async Task<string> ConSume(CustomeContent<string> Context)
        {
            await Console.Out.WriteLineAsync(Context.Context);
            return null;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Register.RegisterQueue("124.70.12.71", "jorenoe", "jorenoe", "/", "hell");
            QueueManager.Receive(new PhoneStore());
            Console.ReadLine();
        }
    }

}
