namespace OrdersMicroService.Core.RabbitMQ;

public record ProductNameUpdateMessage
{
    public Guid ProductID { get; set; }
    public string? NewName { get; set; }
}
