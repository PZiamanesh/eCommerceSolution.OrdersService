using MongoDB.Driver;
using OrdersMicroservice.Core.Entities;
using OrdersMicroservice.Core.RepositoryContracts;
using System.Linq.Expressions;

namespace OrdersMicroservice.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private IMongoCollection<Order> _orders;
    private readonly string collectionName = "orders";

    public OrderRepository(IMongoDatabase mongoDatabase)
    {
        _orders = mongoDatabase.GetCollection<Order>(collectionName);
    }

    public async Task<Order> AddOrder(Order order)
    {
        order.OrderID = Guid.NewGuid();
        order._id = order.OrderID;

        foreach (var item in order.OrderItems)
        {
            item._id = Guid.NewGuid();
        }

        await _orders.InsertOneAsync(order);
        return order;
    }

    public async Task<bool> DeleteOrder(Guid orderId)
    {
        var filter = Builders<Order>.Filter.Eq(order => order.OrderID, orderId);
        var result = await _orders.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }

    public async Task<Order?> GetOrderBy(FilterDefinition<Order> filter)
    {
        var cursor = await _orders.FindAsync(filter);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        var cursor = await _orders.FindAsync(Builders<Order>.Filter.Empty);
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersBy(FilterDefinition<Order> filter)
    {
        var cursor = await _orders.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task<Order> UpdateOrder(Order order)
    {
        var filter = Builders<Order>.Filter.Eq(order => order.OrderID, order.OrderID);
        await _orders.ReplaceOneAsync(filter, order);

        return order;
    }
}
