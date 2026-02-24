using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consumer.Services;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OrderService.Controllers;
using OrderService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Services;

public class RabbitMQConsumer : BackgroundService, IRabbitMQConsumer
{
	private readonly IConnection _connection;

	private readonly IModel _channel;

public RabbitMQConsumer()
{
    var factory = new ConnectionFactory
    {
        HostName = (Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"),
        Port = 5672,
        UserName = (Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest"),
        Password = (Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest")
    };

    // Retry loop to wait for RabbitMQ to be ready
    int retryCount = 0;
    while (true)
    {
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            break; // Connection successful, exit loop
        }
        catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
        {
            retryCount++;
            if (retryCount > 10) throw; // Fail after 10 attempts
            Console.WriteLine($"RabbitMQ not ready... Retrying in 5s (Attempt {retryCount})");
            Thread.Sleep(5000); 
        }
    }

    _channel.ExchangeDeclare(ChannelSettings.ExchangeName, "direct");
    _channel.QueueDeclare(ChannelSettings.QueueName, durable: true, exclusive: false, autoDelete: false);
    _channel.QueueBind(ChannelSettings.QueueName, ChannelSettings.ExchangeName, ChannelSettings.RoutingKey);
}
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		StartConsuming();
		return Task.CompletedTask;
	}

	public void StartConsuming()
	{
		EventingBasicConsumer consumer = new(_channel);
        consumer.Received += static delegate (object model, BasicDeliverEventArgs ea)
		{
			byte[] bytes = ea.Body.ToArray();
			string value = Encoding.UTF8.GetString(bytes);
			Order order = JsonConvert.DeserializeObject<Order>(value);
			if (order.Status == "new")
			{
				order.ShippingCost = order.TotalAmount * 0.02m;
				OrderController.StoreOrder(order);
			}
		};
		_channel.BasicConsume(ChannelSettings.QueueName, autoAck: true, consumer);
	}

	public override void Dispose()
	{
		_channel.Close();
		_connection.Close();
		base.Dispose();
	}
}
