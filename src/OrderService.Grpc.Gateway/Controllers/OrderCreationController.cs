using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using OrderService.Grpc.Gateway.Models.OrderCreation;
using Swashbuckle.AspNetCore.Annotations;
using OrderCreationService = Orders.CreationService.Contracts.OrderService;
using Pb = Orders.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderCreationController : ControllerBase
{
    private readonly OrderCreationService.OrderServiceClient _orderService;

    public OrderCreationController(OrderCreationService.OrderServiceClient orderService)
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
        [FromBody] IEnumerable<AddOrderModel> orders,
        CancellationToken cancellationToken)
    {
        IEnumerable<Pb.AddOrderDto> pbOrders = orders
            .Select(x => new Pb.AddOrderDto { OrderCreatedBy = x.OrderCreatedBy });

        var request = new Pb.AddOrdersRequest { Orders = { pbOrders } };
        Pb.AddOrdersResponse response = await _orderService.AddOrdersAsync(request, cancellationToken: cancellationToken);

        return Ok(response.OrdersIds.ToArray());
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
        [FromBody] IEnumerable<AddProductToOrderModel> products,
        CancellationToken cancellationToken)
    {
        IEnumerable<Pb.AddProductToOrderDto> pbProducts = products.Select(x =>
                new Pb.AddProductToOrderDto { ProductId = x.ProductId, Quantity = x.Quantity });

        var request = new Pb.AddProductsToOrderRequest { OrderId = orderId, Products = { pbProducts } };
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
        [FromBody] IEnumerable<long> productIds,
        CancellationToken cancellationToken)
    {
        var request = new Pb.RemoveProductsFromOrderRequest { OrderId = orderId, ProductIds = { productIds } };
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
        var request = new Pb.StartOrderProcessingRequest { OrderId = orderId };
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
        var request = new Pb.CancelOrderRequest { OrderId = orderId };
        await _orderService.CancelOrderAsync(request, cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Query orders with filters
    /// </summary>
    /// <returns>An array of orders matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Orders queried", typeof(Order[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying orders")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet]
    public async Task<ActionResult<Order[]>> QueryOrdersAsync(
        [FromQuery] OrderQuery query,
        CancellationToken cancellationToken)
    {
        var q = new Pb.OrderQuery
        {
            Ids = { query.Ids },
            OrderState = query.OrderState ?? Pb.OrderState.Unspecified,
            CreatedBy = query.CreatedBy,
            Cursor = query.Cursor,
            PageSize = query.PageSize,
        };

        AsyncServerStreamingCall<Pb.OrderDto> response = _orderService.QueryOrders(q, cancellationToken: cancellationToken);

        var orders = new List<Order>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            Pb.OrderDto o = response.ResponseStream.Current;
            orders.Add(new Order(o.OrderState, o.OrderCreatedAt.ToDateTimeOffset(), o.OrderCreatedBy));
        }

        return Ok(orders);
    }

    /// <summary>
    /// Query order items with filters
    /// </summary>
    /// <returns>An array of order items matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Order items queried", typeof(OrderItem[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying order items")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet("items")]
    public async Task<ActionResult<OrderItem[]>> QueryOrderItemsAsync(
        [FromQuery] OrderItemQuery query,
        CancellationToken cancellationToken)
    {
        var q = new Pb.OrderItemQuery
        {
            Ids = { query.Ids },
            OrderIds = { query.OrderIds ?? [] },
            ProductIds = { query.ProductIds ?? [] },
            Deleted = query.Deleted,
            Cursor = query.Cursor,
            PageSize = query.PageSize,
        };

        AsyncServerStreamingCall<Pb.OrderItemDto> response = _orderService.QueryItems(q, cancellationToken: cancellationToken);

        var items = new List<OrderItem>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            Pb.OrderItemDto item = response.ResponseStream.Current;
            items.Add(new OrderItem(
                OrderId: item.OrderId,
                ProductId: item.ProductId,
                Quantity: item.OrderItemQuantity));
        }

        return Ok(items);
    }

    /// <summary>
    /// Query order history with filters
    /// </summary>
    /// <returns>An array of order history entries matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Order history queried", typeof(OrderHistoryItem[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying order history")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet("history")]
    public async Task<ActionResult<Pb.OrderHistoryItemDto[]>> QueryOrderHistoryAsync(
        [FromQuery] long[] ids,
        [FromQuery] long[]? orderIds,
        [FromQuery] Pb.OrderHistoryItemKind? itemKind,
        [FromQuery] int cursor,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var q = new Pb.OrderHistoryQuery
        {
            Ids = { ids },
            OrderIds = { orderIds ?? [] },
            Kind = itemKind ?? Pb.OrderHistoryItemKind.Unspecified,
            Cursor = cursor,
            PageSize = pageSize,
        };

        AsyncServerStreamingCall<Pb.OrderHistoryItemDto> response = _orderService.QueryHistory(q, cancellationToken: cancellationToken);

        var historyItems = new List<OrderHistoryItem>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            Pb.OrderHistoryItemDto item = response.ResponseStream.Current;
            historyItems.Add(new OrderHistoryItem(
                OrderId: item.OrderId,
                CreatedAt: item.OrderHistoryItemCreatedAt.ToDateTimeOffset(),
                Kind: item.OrderHistoryItemKind,
                Payload: item.Payload));
        }

        return Ok(historyItems);
    }
}