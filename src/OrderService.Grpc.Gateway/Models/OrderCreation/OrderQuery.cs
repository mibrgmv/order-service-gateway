namespace OrderService.Grpc.Gateway.Models.OrderCreation;

public sealed record OrderQuery(
    long[] Ids,
    OrderState? OrderState,
    string? CreatedBy,
    int Cursor,
    int PageSize);
