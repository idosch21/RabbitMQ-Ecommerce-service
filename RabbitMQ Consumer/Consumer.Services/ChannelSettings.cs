namespace Consumer.Services;

public class ChannelSettings
{
	private static readonly string exchangeName = "ExchangeForEx1";

	private static readonly string queueName = "QueueForExercise1";

	private static readonly string routingKey = "MyKey";

	public static string ExchangeName => exchangeName;

	public static string QueueName => queueName;

	public static string RoutingKey => routingKey;
}
