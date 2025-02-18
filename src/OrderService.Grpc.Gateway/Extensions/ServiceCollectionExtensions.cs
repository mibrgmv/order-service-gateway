using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orders.CreationService.Contracts;
using Orders.ProcessingService.Contracts;
using OrderService.Grpc.Gateway.Options;
using Products.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<ProductServiceOptions>().BindConfiguration(nameof(ProductServiceOptions));
        serviceCollection.AddOptions<OrderCreationServiceOptions>().BindConfiguration(nameof(OrderCreationServiceOptions));
        serviceCollection.AddOptions<OrderProcessingServiceOptions>().BindConfiguration(nameof(OrderProcessingServiceOptions));

        serviceCollection.AddGrpcClient<ProductService.ProductServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<ProductServiceOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<ProductServiceOptions>>();
            op.Address = new Uri(snapshot.Value.Address);
        });

        serviceCollection.AddGrpcClient<OrderCreationService.OrderCreationServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<OrderCreationServiceOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<OrderCreationServiceOptions>>();
            op.Address = new Uri(snapshot.Value.Address);
        });

        serviceCollection.AddGrpcClient<OrderProcessingService.OrderProcessingServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<OrderProcessingServiceOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<OrderProcessingServiceOptions>>();
            op.Address = new Uri(snapshot.Value.Address);
        });

        return serviceCollection;
    }
}