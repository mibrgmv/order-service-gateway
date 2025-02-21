using Orders.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public readonly record struct Order(OrderState OrderState, DateTimeOffset OrderCreatedAt, string OrderCreatedBy);