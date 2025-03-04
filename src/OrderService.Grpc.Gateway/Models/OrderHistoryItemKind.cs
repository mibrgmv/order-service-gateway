namespace OrderService.Grpc.Gateway.Models;

public enum OrderHistoryItemKind
{
    Unspecified = 0,
    Created = 1,
    ItemAdded = 2,
    ItemRemoved = 3,
    StateChanged = 4,
}