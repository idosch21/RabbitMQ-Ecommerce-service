namespace ProducerService_Exercise1.Connection;

public interface IMessageProducer
{
	void PublishMessage<T>(T message);
}
