namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public enum OrderState
{
    Created = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4,
}