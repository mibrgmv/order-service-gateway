namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public sealed record OrderItemQuery(
    long[]? Ids,
    long[]? OrderIds,
    long[]? ProductIds,
    bool? Deleted,
    int Cursor,
    int PageSize);