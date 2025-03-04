namespace OrdersMicroservice.Core.DTOs;

public record OrderResponse
{
    public Guid OrderID { get; set; }
    public Guid UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalBill { get; set; }
    public List<OrderItemResponse> OrderItems { get; set; }
}
