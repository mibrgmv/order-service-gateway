using Microsoft.AspNetCore.Mvc;
using Orders.ProcessingService.Contracts;
using OrderProcessingService = Orders.ProcessingService.Contracts.OrderService;

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

    [HttpPost("{orderId:long}/approve")]
    public async Task<ActionResult> ApproveOrder(
        [FromRoute] long orderId,
        [FromQuery] bool isApproved,
        [FromQuery] string approvedBy,
        [FromQuery] string? failureReason = null)
    {
        var request = new ApproveOrderRequest
        {
            OrderId = orderId, IsApproved = isApproved, ApprovedBy = approvedBy, FailureReason = failureReason,
        };

        await _service.ApproveOrderAsync(request);
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
