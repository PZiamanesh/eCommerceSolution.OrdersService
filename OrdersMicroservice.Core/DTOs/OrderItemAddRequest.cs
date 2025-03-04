namespace OrdersMicroservice.Core.DTOs;

public record OrderItemAddRequest
{
    public Guid ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}