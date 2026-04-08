using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.DTOs;
using System.Net.Http.Json;

namespace SportsStore.Pages
{
    public class OrderStatusModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrderStatusModel> _logger;

        public OrderStatusModel(IHttpClientFactory httpClientFactory,
            ILogger<OrderStatusModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public Guid OrderId { get; set; }

        public OrderDto? Order { get; set; }
        public bool Loading { get; set; } = true;

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("OrderAPI");
                Order = await client.GetFromJsonAsync<OrderDto>($"api/orders/{OrderId}");
                _logger.LogInformation("Order status retrieved for {OrderId}", OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order status for {OrderId}", OrderId);
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