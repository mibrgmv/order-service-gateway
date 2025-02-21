using Orders.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public readonly record struct OrderHistoryItem(
    long OrderId,
    DateTimeOffset CreatedAt,
    OrderHistoryItemKind Kind,
    BasePayload Payload);