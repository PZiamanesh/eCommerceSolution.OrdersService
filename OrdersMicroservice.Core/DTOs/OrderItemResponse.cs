﻿namespace OrdersMicroservice.Core.DTOs;

public record OrderItemResponse
{
    public Guid ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
}