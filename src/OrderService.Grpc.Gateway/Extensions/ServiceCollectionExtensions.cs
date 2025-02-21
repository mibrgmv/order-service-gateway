using Microsoft.Extensions.Options;
using OrderService.Grpc.Gateway.Options;
using Products.CreationService.Contracts;
using OrderCreationService = Orders.CreationService.Contracts.OrderService;
using OrderProcessingService = Orders.ProcessingService.Contracts.OrderService;

namespace OrderService.Grpc.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<GrpcServerOptions>("order-creation")
            .BindConfiguration("OrderCreationService");

        serviceCollection
            .AddOptions<GrpcServerOptions>("order-processing")
            .BindConfiguration("OrderProcessingService");

        serviceCollection
            .AddOptions<GrpcServerOptions>("product")
            .BindConfiguration("ProductService");

        serviceCollection.AddGrpcClient<OrderCreationService.OrderServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<GrpcServerOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<GrpcServerOptions>>();
            GrpcServerOptions options = snapshot.Get("order-creation");
            op.Address = new Uri(options.Address);
        });

        serviceCollection.AddGrpcClient<OrderProcessingService.OrderServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<GrpcServerOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<GrpcServerOptions>>();
            GrpcServerOptions options = snapshot.Get("order-processing");
            op.Address = new Uri(options.Address);
        });

        serviceCollection.AddGrpcClient<ProductService.ProductServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<GrpcServerOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<GrpcServerOptions>>();
            GrpcServerOptions options = snapshot.Get("product");
            op.Address = new Uri(options.Address);
        });

        return serviceCollection;
    }
}