namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public readonly record struct AddProductToOrderModel(long ProductId, int Quantity);