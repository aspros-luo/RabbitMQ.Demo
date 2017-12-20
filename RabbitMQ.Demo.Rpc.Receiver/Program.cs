using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Demo.Rpc.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
       
            var eventingBasicConsumer = new EventingBasicConsumer(channel);
            eventingBasicConsumer.Received += (model, ea) =>
            {
                var msg = Encoding.UTF8.GetString(ea.Body);
                channel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("message:{0}", msg);
                Console.WriteLine("enter your message and press Enter!");
                var response = Console.ReadLine();
                var replyBasicPorperties = channel.CreateBasicProperties();
                replyBasicPorperties.CorrelationId = ea.BasicProperties.CorrelationId;
                var msgByte = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish("", ea.BasicProperties.ReplyTo, replyBasicPorperties, msgByte);
            };
            channel.BasicConsume("my queue rpc", false, eventingBasicConsumer);
        }
    }
}
