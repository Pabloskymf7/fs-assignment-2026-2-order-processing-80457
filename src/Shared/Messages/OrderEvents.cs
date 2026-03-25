namespace Shared.Messages;

// ── Orden enviada desde el checkout ──────────────────────────────────────────
public record OrderSubmitted(
    Guid OrderId,
    Guid CustomerId,
    List<OrderItemMessage> Items,
    decimal TotalAmount,
    DateTime SubmittedAt
);

// ── Inventario ────────────────────────────────────────────────────────────────
public record InventoryCheckRequested(
    Guid OrderId,
    List<OrderItemMessage> Items
);

public record InventoryConfirmed(Guid OrderId);

public record InventoryFailed(Guid OrderId, string Reason);

// ── Pago ─────────────────────────────────────────────────────────────────────
public record PaymentProcessingRequested(
    Guid OrderId,
    Guid CustomerId,
    decimal Amount
);

public record PaymentApproved(Guid OrderId, string TransactionId);

public record PaymentRejected(Guid OrderId, string Reason);

// ── Envío ─────────────────────────────────────────────────────────────────────
public record ShippingRequested(
    Guid OrderId,
    Guid CustomerId
);

public record ShippingCreated(
    Guid OrderId,
    string TrackingReference,
    DateTime EstimatedDispatch
);

public record ShippingFailed(Guid OrderId, string Reason);

// ── Resultado final ───────────────────────────────────────────────────────────
public record OrderCompleted(Guid OrderId);

public record OrderFailed(Guid OrderId, string Reason);