namespace OrdersMicroservice.Core.DTOs;

public record OrderAddRequest
{
    public Guid UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemAddRequest> OrderItems { get; set; }
}
