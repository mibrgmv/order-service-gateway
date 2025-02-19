namespace OrderService.Grpc.Gateway.Models.Products;

public sealed record ProductQuery(
    long[] Ids,
    string? NamePattern,
    double? MinPrice,
    double? MaxPrice,
    int Cursor,
    int PageSize);