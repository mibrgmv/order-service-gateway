namespace OrderService.Grpc.Gateway.Options;

public sealed class GrpcServerOptions
{
    private const string SectionName = "Grpc";

    public static string ProductServiceSection => SectionName + "ProductService";

    public static string OrderCreationServiceSection => SectionName + "OrderService";

    public static string OrderProcessingServiceSection => SectionName + "OrderProcessingService";

    public string Prefix { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Address => $"{Prefix}://{Host}:{Port}";
}