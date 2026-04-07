using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public class ProcessPaymentResultCommandHandler : IRequestHandler<ProcessPaymentResultCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProcessPaymentResultCommandHandler> _logger;

    public ProcessPaymentResultCommandHandler(AppDbContext db,
        ILogger<ProcessPaymentResultCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(ProcessPaymentResultCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) return;

        order.Status = request.Approved ? OrderStatus.PaymentApproved : OrderStatus.PaymentFailed;
        order.FailureReason = request.Reason;
        order.UpdatedAt = DateTime.UtcNow;

        if (request.Approved)
        {
            order.Payment = new PaymentRecord
            {
                OrderId = order.Id,
                TransactionId = request.TransactionId,
                Approved = true
            };
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Payment result processed for order {OrderId} {EventType} approved={Approved}",
            request.OrderId, "ProcessPaymentResult", request.Approved);
    }
}
