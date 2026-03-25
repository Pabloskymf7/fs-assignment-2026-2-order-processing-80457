using Shared.Enums;

namespace Shared.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? TrackingReference { get; set; }
    public DateTime? EstimatedDispatch { get; set; }
    public string? FailureReason { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public PaymentDto? Payment { get; set; }
    public ShipmentDto? Shipment { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PaymentDto
{
    public string? TransactionId { get; set; }
    public bool Approved { get; set; }
    public string? Reason { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ShipmentDto
{
    public string TrackingReference { get; set; } = string.Empty;
    public DateTime EstimatedDispatch { get; set; }
    public DateTime CreatedAt { get; set; }
}