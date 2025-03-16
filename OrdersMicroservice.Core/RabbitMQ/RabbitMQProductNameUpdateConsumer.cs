using DnsClient.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrdersMicroService.Core.RabbitMQ;

public class RabbitMQProductNameUpdateConsumer : IDisposable, IRabbitMQProductNameUpdateConsumer
{
    private readonly IConfiguration configuration;
    private readonly ILogger<RabbitMQProductNameUpdateConsumer> logger;
    private readonly IConnection connection;
    private readonly IModel channel;

    public RabbitMQProductNameUpdateConsumer(
        IConfiguration configuration,
        ILogger<RabbitMQProductNameUpdateConsumer> logger
        )
    {
        this.configuration = configuration;
        this.logger = logger;
        var hostname = this.configuration["RabbitMQ_HostName"]!;
        var userName = this.configuration["RabbitMQ_UserName"]!;
        var password = this.configuration["RabbitMQ_Password"]!;
        var port = this.configuration["RabbitMQ_Port"]!;

        var connectionFactory = new ConnectionFactory()
        {
            HostName = hostname,
            UserName = userName,
            Password = password,
            Port = int.Parse(port)
        };

        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
    }

    public void Consume()
    {
        string routingKey = "product.update.name";
        string queueName = "product.update.name.queue";
        string exchangeName = configuration["RabbitMQ_Products_Exchange"]!;

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queueName, exchangeName, routingKey);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (sender, args) =>
        {
            byte[] body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            if (message != null)
            {
                var productNameUpdateMessage = JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);
                logger.LogInformation($"product name updated: {productNameUpdateMessage.ProductID} - {productNameUpdateMessage.ProductName}");
            }
        };

        channel.BasicConsume(queueName, consumer: consumer, autoAck: true);
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}