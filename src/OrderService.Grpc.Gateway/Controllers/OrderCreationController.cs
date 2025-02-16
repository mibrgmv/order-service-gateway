using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.CreationService.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderService.Grpc.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderCreationController : ControllerBase
{
    private readonly OrderCreationService.OrderCreationServiceClient _orderService;

    public OrderCreationController(OrderCreationService.OrderCreationServiceClient orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Add an array of orders to the system
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK, "Orders added", typeof(long[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while adding orders")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpPut]
    public async Task<ActionResult> AddOrdersAsync(
        [FromBody] RepeatedField<AddOrderDto> orders,
        CancellationToken cancellationToken)
    {
        var request = new AddOrdersRequest { Orders = { orders } };
        AddOrdersResponse response = await _orderService.AddOrdersAsync(request, cancellationToken: cancellationToken);
        return Ok(response.OrdersIds);
    }

    /// <summary>
    /// Add products to an order by id
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK, "Products added")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while adding products to order")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Invalid order state")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpPost("{orderId:long}/products")]
    public async Task<ActionResult> AddProductsToOrderAsync(
        [FromRoute] long orderId,
        [FromBody] RepeatedField<AddProductToOrderDto> products,
        CancellationToken cancellationToken)
    {
        var request = new AddProductsToOrderRequest { OrderId = orderId, Products = { products } };
        await _orderService.AddProductsToOrderAsync(request, cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Remove products from an order by id
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK, "Products removed")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while removing products from order")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Invalid order state")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpDelete("{orderId:long}/products")]
    public async Task<ActionResult> RemoveProductsFromOrderAsync(
        [FromRoute] long orderId,
        [FromBody] RepeatedField<long> productIds,
        CancellationToken cancellationToken)
    {
        var request = new RemoveProductsFromOrderRequest { OrderId = orderId, ProductIds = { productIds } };
        await _orderService.RemoveProductsFromOrderAsync(request, cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Start order processing
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK, "State changed to Processing")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpPost("{orderId:long}/start-processing")]
    public async Task<ActionResult> StartOrderProcessingAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        var request = new StartOrderProcessingRequest { OrderId = orderId };
        await _orderService.StartOrderProcessingAsync(request, cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK, "State changed to Cancelled")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpPost("{orderId:long}/cancel")]
    public async Task<ActionResult> CancelOrderStateAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        var request = new CancelOrderRequest { OrderId = orderId };
        await _orderService.CancelOrderAsync(request, cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Query orders with filters
    /// </summary>
    /// <returns>An array of orders matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Orders queried", typeof(OrderDto[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying orders")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet]
    public async Task<ActionResult<OrderDto[]>> QueryOrdersAsync(
        [FromQuery] OrderQuery query,
        CancellationToken cancellationToken)
    {
        AsyncServerStreamingCall<OrderDto> response = _orderService.QueryOrders(query, cancellationToken: cancellationToken);

        var orders = new List<OrderDto>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            OrderDto order = response.ResponseStream.Current;
            orders.Add(order);
        }

        return Ok(orders);
    }

    /// <summary>
    /// Query order items with filters
    /// </summary>
    /// <returns>An array of order items matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Order items queried", typeof(OrderItemDto[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying order items")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet("items")]
    public async Task<ActionResult<OrderItemDto[]>> QueryOrderItemsAsync(
        [FromQuery] OrderItemQuery query,
        CancellationToken cancellationToken)
    {
        AsyncServerStreamingCall<OrderItemDto> response = _orderService.QueryItems(query, cancellationToken: cancellationToken);

        var items = new List<OrderItemDto>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            OrderItemDto orderItem = response.ResponseStream.Current;
            items.Add(orderItem);
        }

        return Ok(items);
    }

    /// <summary>
    /// Query order history with filters
    /// </summary>
    /// <returns>An array of order history entries matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Order history queried", typeof(OrderHistoryItemDto[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying order history")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet("history")]
    public async Task<ActionResult<OrderHistoryItemDto[]>> QueryOrderHistoryAsync(
        [FromQuery] OrderHistoryQuery query,
        CancellationToken cancellationToken)
    {
        AsyncServerStreamingCall<OrderHistoryItemDto> response = _orderService.QueryHistory(query, cancellationToken: cancellationToken);

        var historyItems = new List<OrderHistoryItemDto>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            OrderHistoryItemDto item = response.ResponseStream.Current;
            historyItems.Add(item);
        }

        return Ok(historyItems);
    }
}