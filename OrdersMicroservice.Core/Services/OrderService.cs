using AutoMapper;
using MongoDB.Driver;
using OrdersMicroservice.Core.DTOs;
using OrdersMicroservice.Core.Entities;
using OrdersMicroservice.Core.HttpClients;
using OrdersMicroservice.Core.RepositoryContracts;
using OrdersMicroservice.Core.ServiceContracts;
using OrdersMicroservice.Core.Validators;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrdersMicroservice.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;
    private readonly IMapper mapper;
    private readonly UsersMicroserviceClient usersMicroserviceClient;
    private readonly ProductsMicroserviceClient productsMicroserviceClient;

    public OrderService(
        IOrderRepository orderRepository,
        IMapper mapper,
        UsersMicroserviceClient usersMicroserviceClient,
        ProductsMicroserviceClient productsMicroserviceClient
        )
    {
        this.orderRepository = orderRepository;
        this.mapper = mapper;
        this.usersMicroserviceClient = usersMicroserviceClient;
        this.productsMicroserviceClient = productsMicroserviceClient;
    }

    public async Task<OrderResponse> AddOrder(OrderAddRequest orderAddRequest)
    {
        if (orderAddRequest == null)
        {
            throw new ArgumentNullException(nameof(orderAddRequest));
        }

        var userDto = await usersMicroserviceClient.GetUserByUserId(orderAddRequest.UserID);

        if (userDto is null)
        {
            throw new ArgumentNullException("Invalid userId");
        }

        List<ProductDTO?> products = new List<ProductDTO?>();

        foreach (var orderItem in orderAddRequest.OrderItems)
        {
            var productDto = await productsMicroserviceClient.GetProductByProductId(orderItem.ProductID);

            if (productDto is null)
            {
                throw new ArgumentException("Invalid productId");
            }

            products.Add(productDto);
        }

        Order orderInput = mapper.Map<Order>(orderAddRequest);

        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }

        orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);

        Order addedOrder = await orderRepository.AddOrder(orderInput);

        OrderResponse addedOrderResponse = mapper.Map<OrderResponse>(addedOrder);

        foreach (var orderItem in addedOrderResponse.OrderItems)
        {
            var productDto = products.FirstOrDefault(p => p.ProductID == orderItem.ProductID);
            if (productDto is null) continue;
            mapper.Map<ProductDTO, OrderItemResponse>(productDto, orderItem);
        }
        mapper.Map<UserDTO, OrderResponse>(userDto, addedOrderResponse);

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

        if (orderResponse is not null)
        {
            foreach (var orderItem in orderResponse.OrderItems)
            {
                var productDto = await productsMicroserviceClient.GetProductByProductId(orderItem.ProductID);
                if (productDto is null) continue;
                mapper.Map<ProductDTO, OrderItemResponse>(productDto, orderItem);
            }

            var userDto = await usersMicroserviceClient.GetUserByUserId(orderResponse.UserID);
            if (userDto != null) mapper.Map<UserDTO, OrderResponse>(userDto, orderResponse);
        }

        return orderResponse;
    }

    public async Task<List<OrderResponse>> GetOrders()
    {
        IEnumerable<Order> orders = await orderRepository.GetOrders();
        IEnumerable<OrderResponse> orderResponses = mapper.Map<IEnumerable<OrderResponse>>(orders);

        foreach (var orderResponse in orderResponses)
        {
            if (orderResponse is null)
            {
                continue;
            }

            foreach (var orderItem in orderResponse.OrderItems)
            {
                var productDto = await productsMicroserviceClient.GetProductByProductId(orderItem.ProductID);
                if (productDto is null) continue;
                mapper.Map<ProductDTO, OrderItemResponse>(productDto, orderItem);
            }

            var userDto = await usersMicroserviceClient.GetUserByUserId(orderResponse.UserID);
            if (userDto != null) mapper.Map<UserDTO, OrderResponse>(userDto, orderResponse);
        }

        return orderResponses.ToList();
    }

    public async Task<List<OrderResponse>> GetOrdersBy(FilterDefinition<Order> filter)
    {
        IEnumerable<Order> orders = await orderRepository.GetOrdersBy(filter);
        IEnumerable<OrderResponse> orderResponses = mapper.Map<IEnumerable<OrderResponse>>(orders);

        foreach (var orderResponse in orderResponses)
        {
            if (orderResponse is null)
            {
                continue;
            }

            foreach (var orderItem in orderResponse.OrderItems)
            {
                var productDto = await productsMicroserviceClient.GetProductByProductId(orderItem.ProductID);
                if (productDto is null) continue;
                mapper.Map<ProductDTO, OrderItemResponse>(productDto, orderItem);
            }

            var userDto = await usersMicroserviceClient.GetUserByUserId(orderResponse.UserID);
            if (userDto != null) mapper.Map<UserDTO, OrderResponse>(userDto, orderResponse);
        }

        return orderResponses.ToList();
    }

    public async Task<OrderResponse> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
    {
        if (orderUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(orderUpdateRequest));
        }

        var userDto = await usersMicroserviceClient.GetUserByUserId(orderUpdateRequest.UserID);

        if (userDto is null)
        {
            throw new ArgumentNullException("Invalid userId");
        }

        List<ProductDTO?> products = new List<ProductDTO?>();

        foreach (var orderItem in orderUpdateRequest.OrderItems)
        {
            var productDto = await productsMicroserviceClient.GetProductByProductId(orderItem.ProductID);

            if (productDto is null)
            {
                throw new ArgumentException("Invalid productId");
            }

            products.Add(productDto);
        }

        Order orderInput = mapper.Map<Order>(orderUpdateRequest);

        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);

        Order updatedOrder = await orderRepository.UpdateOrder(orderInput);

        OrderResponse updatedOrderResponse = mapper.Map<OrderResponse>(updatedOrder);

        foreach (var orderItem in updatedOrderResponse.OrderItems)
        {
            var productDto = await productsMicroserviceClient.GetProductByProductId(orderItem.ProductID);
            if (productDto is null) continue;
            mapper.Map<ProductDTO, OrderItemResponse>(productDto, orderItem);
        }
        mapper.Map<UserDTO, OrderResponse>(userDto, updatedOrderResponse);

        return updatedOrderResponse;
    }
}
