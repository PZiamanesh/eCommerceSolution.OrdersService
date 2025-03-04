namespace OrdersMicroservice.Core.DTOs;

public record OrderItemUpdateRequest
{
    public Guid ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
