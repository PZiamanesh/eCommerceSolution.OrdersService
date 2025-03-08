namespace OrdersMicroservice.Core.DTOs;

public record ProductDTO
{
    public Guid ProductID { get; init; }
    public string? ProductName { get; init; }
    public string? Category { get; init; }
    public double UnitPrice { get; init; }
    public int QuantityInStock { get; init; }
}