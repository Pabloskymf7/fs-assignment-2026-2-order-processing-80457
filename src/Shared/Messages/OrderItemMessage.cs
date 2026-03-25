namespace Shared.Messages;

public record OrderItemMessage(
	int ProductId,
	string ProductName,
	int Quantity,
	decimal UnitPrice
);