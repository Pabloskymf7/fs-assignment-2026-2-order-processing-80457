using MediatR;

namespace OrderManagement.API.Application.Commands;

public record ProcessPaymentResultCommand(
    Guid OrderId,
    bool Approved,
    string? TransactionId = null,
    string? Reason = null
) : IRequest;
