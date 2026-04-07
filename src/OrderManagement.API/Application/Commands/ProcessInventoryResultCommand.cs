using MediatR;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public record ProcessInventoryResultCommand(
    Guid OrderId,
    bool Confirmed,
    string? Reason = null
) : IRequest;
