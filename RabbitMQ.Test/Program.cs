using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RunRpcQueue();
        }

        private static void SendQueue()
        {
            var connectionFactory = new ConnectionFactory
            {
                //                Port = 5672,
                HostName = "localhost",
                //                UserName = "accountant",
                //                Password = "accointant",
                //                VirtualHost = "accounting"
            };
            const string exchangeName = "my fanout exchange";
            const string queueName = "first queue";
            const string queueMessage = "first queue message";
            const string queueName1 = "secend queue";
            //            const string queueMessage1 = "secend queue message";
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            Console.WriteLine(string.Concat("connection open:", connection.IsOpen));
            //声明交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true, false, null);
            //声明消息队列
            channel.QueueDeclare(queueName, true, false, false, null);
            //绑定交换机与消息队列
            channel.QueueBind(queueName, exchangeName, "");

            channel.QueueDeclare(queueName1, true, false, false, null);
            channel.QueueBind(queueName1, exchangeName, "");

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            var address = new PublicationAddress(ExchangeType.Direct, exchangeName, "");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes(queueMessage));
            //
            channel.Close();
            connection.Close();
            Console.WriteLine(string.Concat("channel close:", channel.IsClosed));
            Console.WriteLine("Main more...");
            Console.ReadKey();
        }

        private static void RunRpcQueue()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            const string queueKey = "my queue rpc";
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueKey, true, false, false, null);
            SendRpcMsgBackAndForth(channel, queueKey);
        }

        private static void SendRpcMsgBackAndForth(IModel channel, string queueKey)
        {
            var rpcResponseQueue = channel.QueueDeclare().QueueName;
            var correlationId = Guid.NewGuid().ToString();
            string responseFromConsumer = null;

            var basicPorperties = channel.CreateBasicProperties();
            basicPorperties.ReplyTo = rpcResponseQueue;
            basicPorperties.CorrelationId = correlationId;
            Console.WriteLine("pls enter your message and press Enter!");
            var msg = Console.ReadLine();
            var msgByte = Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish("", queueKey, basicPorperties, msgByte);

            var rpcEventingBasicConsumer = new EventingBasicConsumer(channel);
            rpcEventingBasicConsumer.Received += (model, ea) =>
            {
                var props = ea.BasicProperties;
                if (props != null && props.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body);
                    responseFromConsumer = response;
                }
                channel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("response:{0}", responseFromConsumer);
                Console.WriteLine("enter your message and press Enter!");
                msg = Console.ReadLine();
                msgByte = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish("", queueKey, basicPorperties, msgByte);
            };
            channel.BasicConsume(rpcResponseQueue, false, rpcEventingBasicConsumer);
        }
    }
}
