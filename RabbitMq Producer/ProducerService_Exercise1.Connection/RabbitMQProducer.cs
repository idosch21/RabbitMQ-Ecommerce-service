using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ProducerService_Exercise1.Connection;

public class RabbitMQProducer : IMessageProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQProducer()
    {
        var factory = new ConnectionFactory
        {
            HostName = (Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"),
            Port = 5672,
            UserName = (Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest"),
            Password = (Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest")
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Initial setup in constructor to ensure infrastructure exists on startup
        _channel.ExchangeDeclare(ChannelSettings.ExchangeName, "direct");
        _channel.QueueDeclare(ChannelSettings.QueueName, durable: true, exclusive: false, autoDelete: false, null);
        _channel.QueueBind(ChannelSettings.QueueName, ChannelSettings.ExchangeName, ChannelSettings.RoutingKey, null);
    }

    public void PublishMessage<T>(T message)
    {
        // Re-declaring ensures the "plumbing" is there even if manually deleted in UI
        _channel.ExchangeDeclare(ChannelSettings.ExchangeName, "direct");
        _channel.QueueDeclare(ChannelSettings.QueueName, durable: true, exclusive: false, autoDelete: false, null);
        _channel.QueueBind(ChannelSettings.QueueName, ChannelSettings.ExchangeName, ChannelSettings.RoutingKey, null);

        string json = JsonConvert.SerializeObject(message);
        byte[] body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: ChannelSettings.ExchangeName,
            routingKey: ChannelSettings.RoutingKey,
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        
        if (_channel.IsOpen) _channel.Close();
        if (_connection.IsOpen) _connection.Close();
    }
}