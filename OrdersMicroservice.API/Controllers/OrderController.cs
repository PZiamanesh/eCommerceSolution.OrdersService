using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrdersMicroservice.Core.DTOs;
using OrdersMicroservice.Core.Entities;
using OrdersMicroservice.Core.ServiceContracts;

namespace OrdersMicroservice.API.Controllers;

[ApiController]
[Route("api/orders/")]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders()
    {
        List<OrderResponse> orders = await orderService.GetOrders();
        return Ok(orders);
    }

    [HttpGet("search/orderid/{orderID}")]
    public async Task<ActionResult<OrderResponse?>> GetOrderByOrderID(Guid orderID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);
        OrderResponse? order = await orderService.GetOrderBy(filter);

        if (order is null)
        {
            return BadRequest();
        }

        return Ok(order);
    }

    [HttpGet("search/productid/{productID}")]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersByProductID(Guid productID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(temp => temp.OrderItems,
          Builders<OrderItem>.Filter.Eq(tempProduct => tempProduct.ProductID, productID));

        List<OrderResponse> orders = await orderService.GetOrdersBy(filter);
        return Ok(orders);
    }

    [HttpGet("search/orderDate/{orderDate}")]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersByOrderDate(DateTime orderDate)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderDate.ToString("yyyy-MM-dd"),orderDate.ToString("yyyy-MM-dd"));

        List<OrderResponse> orders = await orderService.GetOrdersBy(filter);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> AddOrder(OrderAddRequest orderAddRequest)
    {
        OrderResponse orderResponse = await orderService.AddOrder(orderAddRequest);

        return Created($"api/orders/search/orderid/{orderResponse.OrderID}", orderResponse);
    }

    [HttpPut("{orderID}")]
    public async Task<IActionResult> UpdateOrder(Guid orderID, OrderUpdateRequest orderUpdateRequest)
    {
        if (orderID != orderUpdateRequest.OrderID)
        {
            return BadRequest("OrderID in the URL doesn't match with the OrderID in the Request body");
        }

        OrderResponse orderResponse = await orderService.UpdateOrder(orderUpdateRequest);

        return Ok(orderResponse);
    }

    [HttpDelete("{orderID}")]
    public async Task<IActionResult> DeleteOrder(Guid orderID)
    {
        bool isDeleted = await orderService.DeleteOrder(orderID);

        if (!isDeleted)
        {
            return Problem("Error in adding product");
        }

        return Ok(isDeleted);
    }
}
