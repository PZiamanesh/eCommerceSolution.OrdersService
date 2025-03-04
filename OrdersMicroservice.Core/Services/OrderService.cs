using AutoMapper;
using MongoDB.Driver;
using OrdersMicroservice.Core.DTOs;
using OrdersMicroservice.Core.Entities;
using OrdersMicroservice.Core.RepositoryContracts;
using OrdersMicroservice.Core.ServiceContracts;
using OrdersMicroservice.Core.Validators;

namespace OrdersMicroservice.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;
    private readonly IMapper mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IMapper mapper
        )
    {
        this.orderRepository = orderRepository;
        this.mapper = mapper;
    }

    public async Task<OrderResponse> AddOrder(OrderAddRequest orderAddRequest)
    {
        //Check for null parameter
        if (orderAddRequest == null)
        {
            throw new ArgumentNullException(nameof(orderAddRequest));
        }

        //Convert data from OrderAddRequest to Order
        Order orderInput = mapper.Map<Order>(orderAddRequest);

        //Generate values
        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);


        //Invoke repository
        Order addedOrder = await orderRepository.AddOrder(orderInput);

        OrderResponse addedOrderResponse = mapper.Map<OrderResponse>(addedOrder);

        return addedOrderResponse;
    }

    public async Task<bool> DeleteOrder(Guid orderID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);
        Order? existingOrder = await orderRepository.GetOrderBy(filter);

        if (existingOrder == null)
        {
            return false;
        }

        bool isDeleted = await orderRepository.DeleteOrder(orderID);
        return isDeleted;
    }

    public async Task<OrderResponse?> GetOrderBy(FilterDefinition<Order> filter)
    {
        Order? order = await orderRepository.GetOrderBy(filter);

        if (order == null)
            return null;

        OrderResponse orderResponse = mapper.Map<OrderResponse>(order);
        return orderResponse;
    }

    public async Task<List<OrderResponse>> GetOrders()
    {
        IEnumerable<Order> orders = await orderRepository.GetOrders();
        IEnumerable<OrderResponse> orderResponses = mapper.Map<IEnumerable<OrderResponse>>(orders);

        return orderResponses.ToList();
    }

    public async Task<List<OrderResponse>> GetOrdersBy(FilterDefinition<Order> filter)
    {
        IEnumerable<Order> orders = await orderRepository.GetOrdersBy(filter);
        IEnumerable<OrderResponse> orderResponses = mapper.Map<IEnumerable<OrderResponse>>(orders);

        return orderResponses.ToList();
    }

    public async Task<OrderResponse> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
    {
        //Check for null parameter
        if (orderUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(orderUpdateRequest));
        }

        //Convert data from OrderUpdateRequest to Order
        Order orderInput = mapper.Map<Order>(orderUpdateRequest);

        //Generate values
        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);


        //Invoke repository
        Order updatedOrder = await orderRepository.UpdateOrder(orderInput);

        OrderResponse addedOrderResponse = mapper.Map<OrderResponse>(updatedOrder);

        return addedOrderResponse;
    }
}
