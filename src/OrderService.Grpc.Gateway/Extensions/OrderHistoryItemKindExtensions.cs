using OrderService.Grpc.Gateway.Models;
using Pb = Orders.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Extensions;

public static class OrderHistoryItemKindExtensions
{
    public static Pb.OrderHistoryItemKind Serialize(this OrderHistoryItemKind? kind)
    {
        return kind switch
        {
            OrderHistoryItemKind.Unspecified => Pb.OrderHistoryItemKind.Unspecified,
            OrderHistoryItemKind.Created => Pb.OrderHistoryItemKind.Created,
            OrderHistoryItemKind.ItemAdded => Pb.OrderHistoryItemKind.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => Pb.OrderHistoryItemKind.ItemRemoved,
            OrderHistoryItemKind.StateChanged => Pb.OrderHistoryItemKind.StateChanged,
            _ => Pb.OrderHistoryItemKind.Unspecified,
        };
    }

    public static OrderHistoryItemKind Deserialize(this Pb.OrderHistoryItemKind kind)
    {
        return kind switch
        {
            Pb.OrderHistoryItemKind.Unspecified => OrderHistoryItemKind.Unspecified,
            Pb.OrderHistoryItemKind.Created => OrderHistoryItemKind.Created,
            Pb.OrderHistoryItemKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
            Pb.OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
            Pb.OrderHistoryItemKind.StateChanged => OrderHistoryItemKind.StateChanged,
            _ => OrderHistoryItemKind.Unspecified,
        };
    }
}