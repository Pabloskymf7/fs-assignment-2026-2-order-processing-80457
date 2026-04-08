using MediatR;

namespace OrderManagement.API.Application.Commands;

public record CreateShipmentCommand(
    Guid OrderId,
    string TrackingReference,
    DateTime EstimatedDispatch
) : IRequest;
