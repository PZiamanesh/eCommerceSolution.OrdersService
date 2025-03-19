namespace OrdersMicroService.Core.RabbitMQ;

public interface IRabbitMQProductNameUpdateConsumer
{
    void Consume();

    void Dispose();
}
