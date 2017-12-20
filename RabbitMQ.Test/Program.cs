using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory
            {
//                Port = 5672,
                HostName = "localhost",
//                UserName = "accountant",
//                Password = "accointant",
//                VirtualHost = "accounting"
            };
            const string exchangeName = "my first  exchange";
            const string queueName = "my first queue";
            const string queueMessage = "my first queue message";
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            Console.WriteLine(string.Concat("connection open:", connection.IsOpen));
            //声明交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            //声明消息队列
            channel.QueueDeclare(queueName, true, false, false, null);
            //绑定交换机与消息队列
            channel.QueueBind(queueName, exchangeName, "");
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            var address = new PublicationAddress(ExchangeType.Direct,exchangeName,"");
            channel.BasicPublish(address,properties,Encoding.UTF8.GetBytes(queueMessage));
            //
            channel.Close();
            connection.Close();
            Console.WriteLine(string.Concat("channel close:", channel.IsClosed));
            Console.WriteLine("Main more...");
            Console.ReadKey();
        }
    }
}
