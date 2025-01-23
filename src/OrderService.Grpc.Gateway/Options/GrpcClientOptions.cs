namespace OrderService.Grpc.Gateway.Options;

public sealed class GrpcClientOptions
{
    public string Prefix { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Address => $"{Prefix}://{Host}:{Port}";
}