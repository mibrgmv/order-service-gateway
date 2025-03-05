namespace OrderService.Grpc.Gateway.Models;

public enum OrderState
{
    Unspecified = 0,
    Created = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4,
}