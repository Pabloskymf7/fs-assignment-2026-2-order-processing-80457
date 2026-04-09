using Shared.Enums;

namespace OrderManagement.API.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Submitted;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? FailureReason { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public PaymentRecord? Payment { get; set; }
    public ShipmentRecord? Shipment { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PaymentRecord
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public string? TransactionId { get; set; }
    public bool Approved { get; set; }
    public string? Reason { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public class ShipmentRecord
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public string TrackingReference { get; set; } = string.Empty;
    public DateTime EstimatedDispatch { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class InventoryRecord
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public bool Confirmed { get; set; }
    public string? Reason { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
