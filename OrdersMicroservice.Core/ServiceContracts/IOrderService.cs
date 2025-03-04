using MongoDB.Driver;
using OrdersMicroservice.Core.DTOs;
using OrdersMicroservice.Core.Entities;

namespace OrdersMicroservice.Core.ServiceContracts;

public interface IOrderService
{
    Task<List<OrderResponse>> GetOrders();

    Task<List<OrderResponse>> GetOrdersBy(FilterDefinition<Order> filter);

    Task<OrderResponse?> GetOrderBy(FilterDefinition<Order> filter);

    Task<OrderResponse> AddOrder(OrderAddRequest orderAddRequest);

    Task<OrderResponse> UpdateOrder(OrderUpdateRequest orderUpdateRequest);

    Task<bool> DeleteOrder(Guid orderID);
}
