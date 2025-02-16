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
        serviceCollection
            .AddOptions<GrpcServerOptions>(GrpcServerOptions.ProductServiceSection)
            .BindConfiguration(GrpcServerOptions.ProductServiceSection);

        serviceCollection
            .AddOptions<GrpcServerOptions>(GrpcServerOptions.OrderCreationServiceSection)
            .BindConfiguration(GrpcServerOptions.OrderCreationServiceSection);

        serviceCollection
            .AddOptions<GrpcServerOptions>(GrpcServerOptions.OrderProcessingServiceSection)
            .BindConfiguration(GrpcServerOptions.OrderProcessingServiceSection);

        serviceCollection.AddGrpcClient<ProductService.ProductServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<GrpcServerOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<GrpcServerOptions>>();
            GrpcServerOptions options = snapshot.Get(GrpcServerOptions.ProductServiceSection);
            op.Address = new Uri(options.Address);
        });

        serviceCollection.AddGrpcClient<OrderCreationService.OrderCreationServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<GrpcServerOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<GrpcServerOptions>>();
            GrpcServerOptions options = snapshot.Get(GrpcServerOptions.OrderCreationServiceSection);
            op.Address = new Uri(options.Address);
        });

        serviceCollection.AddGrpcClient<OrderProcessingService.OrderProcessingServiceClient>((sp, op) =>
        {
            IOptionsSnapshot<GrpcServerOptions> snapshot = sp.GetRequiredService<IOptionsSnapshot<GrpcServerOptions>>();
            GrpcServerOptions options = snapshot.Get(GrpcServerOptions.OrderProcessingServiceSection);
            op.Address = new Uri(options.Address);
        });

        return serviceCollection;
    }
}