using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderService.Grpc.Gateway.Options;
using Products.CreationService.Contracts;
using OrderServiceProto = Orders.CreationService.Contracts.OrderService;

namespace OrderService.Grpc.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddGrpcClient<ProductService.ProductServiceClient>((sp, op) =>
        {
            GrpcClientOptions clientOptions = sp.GetRequiredService<IOptionsSnapshot<GrpcClientOptions>>().Value;
            op.Address = new Uri(clientOptions.Address);
        });

        serviceCollection.AddGrpcClient<OrderServiceProto.OrderServiceClient>((sp, op) =>
        {
            GrpcClientOptions clientOptions = sp.GetRequiredService<IOptionsSnapshot<GrpcClientOptions>>().Value;
            op.Address = new Uri(clientOptions.Address);
        });

        return serviceCollection;
    }
}