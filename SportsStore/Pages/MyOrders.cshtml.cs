using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.DTOs;
using System.Net.Http.Json;

namespace SportsStore.Pages
{
    public class MyOrdersModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MyOrdersModel> _logger;

        public MyOrdersModel(IHttpClientFactory httpClientFactory,
            ILogger<MyOrdersModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public List<OrderDto> Orders { get; set; } = new();
        public bool Loading { get; set; } = true;

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("OrderAPI");
                Orders = await client.GetFromJsonAsync<List<OrderDto>>("api/orders") ?? new();
                _logger.LogInformation("Retrieved {Count} orders {EventType}",
                    Orders.Count, "GetOrders");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
            }
            finally
            {
                Loading = false;
            }
        }

        public string GetStatusColor(string status) => status switch
        {
            "Completed" => "#4caf50",
            "PaymentFailed" or "InventoryFailed" or "Failed" => "#f44336",
            "ShippingCreated" => "#2196f3",
            "PaymentApproved" => "#8bc34a",
            _ => "#ff9800"
        };
    }
}
