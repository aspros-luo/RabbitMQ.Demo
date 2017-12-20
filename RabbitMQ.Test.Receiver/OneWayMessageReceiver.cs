using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace RabbitMQ.Test.Receiver
{
    public class OneWayMessageReceiver : DefaultBasicConsumer
    {
        private readonly IModel _model;

        public OneWayMessageReceiver(IModel model)
        {
            _model = model;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            Console.WriteLine("message received by the consumer!");
            Debug.WriteLine(string.Concat("message received from the exchange:", exchange));
            Debug.WriteLine(string.Concat("content type:", properties.ContentType));
            Debug.WriteLine(string.Concat("consumer tag:", consumerTag));
            Debug.WriteLine(string.Concat("delivery tag:", deliveryTag));
            Debug.WriteLine(string.Concat("message :", Encoding.UTF8.GetString(body)));
            _model.BasicAck(deliveryTag, false);
        }
    }
}
