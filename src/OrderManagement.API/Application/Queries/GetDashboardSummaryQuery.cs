using MediatR;

namespace OrderManagement.API.Application.Queries;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class DashboardSummaryDto
{
    public int TotalOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int FailedOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
