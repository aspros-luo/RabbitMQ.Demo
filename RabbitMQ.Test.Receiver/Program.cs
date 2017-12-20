﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Test.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiverSingleOneWayMessage();
        }

        private static IModel _channel;
        private static void ReceiverSingleOneWayMessage()
        {
            var connetionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = connetionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.BasicQos(0, 1, false);
            var basicConsumer = new EventingBasicConsumer(_channel);
            basicConsumer.Received += (model, e) =>
            {
                //                Debug.WriteLine(string.Concat("message received from the exchange:", e.Exchange));
                //                Debug.WriteLine(string.Concat("content type:", e.BasicProperties.ContentType));
                //                Debug.WriteLine(string.Concat("consumer tag:", e.ConsumerTag));
                //                Debug.WriteLine(string.Concat("delivery tag:", e.DeliveryTag));
                //                Debug.WriteLine(string.Concat("message :", Encoding.UTF8.GetString(e.Body)));
                Console.WriteLine(" Receive Received {0}", Encoding.UTF8.GetString(e.Body));
                _channel.BasicAck(e.DeliveryTag, false);
            };
            _channel.BasicConsume("first queue", false, basicConsumer);
        }
    }
}
