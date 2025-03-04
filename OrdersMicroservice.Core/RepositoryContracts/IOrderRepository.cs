using MongoDB.Driver;
using OrdersMicroservice.Core.Entities;

namespace OrdersMicroservice.Core.RepositoryContracts;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders();

    Task<IEnumerable<Order>> GetOrdersBy(FilterDefinition<Order> filter);

    Task<Order?> GetOrderBy(FilterDefinition<Order> filter);

    Task<Order> AddOrder(Order order);

    Task<Order> UpdateOrder(Order order);

    Task<bool> DeleteOrder(Guid orderId);
}
