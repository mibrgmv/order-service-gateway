using OrderService.Grpc.Gateway.Models;
using Pb = Orders.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Extensions;

public static class OrderStateExtensions
{
    public static Pb.OrderState Serialize(this OrderState? orderState)
    {
        return orderState switch
        {
            OrderState.Unspecified => Pb.OrderState.Unspecified,
            OrderState.Created => Pb.OrderState.Created,
            OrderState.Processing => Pb.OrderState.Processing,
            OrderState.Completed => Pb.OrderState.Completed,
            OrderState.Cancelled => Pb.OrderState.Cancelled,
            _ => Pb.OrderState.Unspecified,
        };
    }

    public static OrderState Deserialize(this Pb.OrderState orderState)
    {
        return orderState switch
        {
            Pb.OrderState.Unspecified => OrderState.Unspecified,
            Pb.OrderState.Created => OrderState.Created,
            Pb.OrderState.Processing => OrderState.Processing,
            Pb.OrderState.Completed => OrderState.Completed,
            Pb.OrderState.Cancelled => OrderState.Cancelled,
            _ => OrderState.Unspecified,
        };
    }
}