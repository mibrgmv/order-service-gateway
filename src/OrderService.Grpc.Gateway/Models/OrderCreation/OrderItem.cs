namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public readonly record struct OrderItem(
    long OrderId,
    long ProductId,
    int Quantity);