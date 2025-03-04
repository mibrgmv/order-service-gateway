using Microsoft.AspNetCore.Mvc;
using Orders.ProcessingService.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using ApproveOrderRequest = OrderService.Grpc.Gateway.Models.OrderProcessing.ApproveOrderRequest;
using OrderProcessingService = Orders.ProcessingService.Contracts.OrderService;
using Pb = Orders.ProcessingService.Contracts;

namespace OrderService.Grpc.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderProcessingController : ControllerBase
{
    private readonly OrderProcessingService.OrderServiceClient _service;

    public OrderProcessingController(OrderProcessingService.OrderServiceClient service)
    {
        _service = service;
    }

    /// <summary>
    /// Approve an order
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    [HttpPost("{orderId:long}/approve")]
    public async Task<ActionResult> ApproveOrder(
        [FromRoute] long orderId,
        [FromQuery] ApproveOrderRequest request,
        CancellationToken cancellationToken)
    {
        var applicationRequest = new Pb.ApproveOrderRequest
        {
            OrderId = orderId,
            IsApproved = request.IsApproved,
            ApprovedBy = request.ApprovedBy,
            FailureReason = request.FailureReason,
        };

        await _service.ApproveOrderAsync(applicationRequest, cancellationToken: cancellationToken);
        return Ok();
    }

    [HttpPost("{orderId:long}/start-packing")]
    public async Task<ActionResult> StartOrderPacking(
        [FromRoute] long orderId,
        [FromQuery] string packingBy)
    {
        var request = new StartOrderPackingRequest { OrderId = orderId, PackingBy = packingBy };
        await _service.StartOrderPackingAsync(request);
        return Ok();
    }

    [HttpPost("{orderId:long}/finish-packing")]
    public async Task<ActionResult> FinishOrderPacking(
        [FromRoute] long orderId,
        [FromQuery] bool isSuccessful,
        [FromQuery] string? failureReason = null)
    {
        var request = new FinishOrderPackingRequest
        {
            OrderId = orderId, IsSuccessful = isSuccessful, FailureReason = failureReason,
        };

        await _service.FinishOrderPackingAsync(request);
        return Ok();
    }

    [HttpPost("{orderId:long}/start-delivery")]
    public async Task<ActionResult> StartOrderDelivery(
        [FromRoute] long orderId,
        [FromQuery] string deliveredBy)
    {
        var request = new StartOrderDeliveryRequest
        {
            OrderId = orderId, DeliveredBy = deliveredBy,
        };

        await _service.StartOrderDeliveryAsync(request);
        return Ok();
    }

    [HttpPost("{orderId:long}/finish-delivery")]
    public async Task<ActionResult> FinishOrderDelivery(
        [FromRoute] long orderId,
        [FromQuery] bool isSuccessful,
        [FromQuery] string? failureReason = null)
    {
        var request = new FinishOrderDeliveryRequest
        {
            OrderId = orderId, IsSuccessful = isSuccessful, FailureReason = failureReason,
        };

        await _service.FinishOrderDeliveryAsync(request);
        return Ok();
    }
}
