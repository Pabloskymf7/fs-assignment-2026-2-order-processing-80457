using MediatR;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    string? FailureReason = null,
    string? TransactionId = null,
    string? TrackingReference = null,
    DateTime? EstimatedDispatch = null
) : IRequest;