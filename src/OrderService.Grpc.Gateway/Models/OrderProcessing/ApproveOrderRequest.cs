namespace OrderService.Grpc.Gateway.Models.OrderProcessing;

public readonly record struct ApproveOrderRequest(
    bool IsApproved,
    string ApprovedBy,
    string? FailureReason = null);