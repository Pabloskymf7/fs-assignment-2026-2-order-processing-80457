using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.DTOs;
using SportsStore.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace SportsStore.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly Cart _cart;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CheckoutModel> _logger;

        public CheckoutModel(Cart cart, IHttpClientFactory httpClientFactory,
            ILogger<CheckoutModel> logger)
        {
            _cart = cart;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Cart Cart => _cart;

        [BindProperty]
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please enter your address")]
        public string Line1 { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please enter your city")]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please enter your country")]
        public string Country { get; set; } = string.Empty;

        public bool OrderPlaced { get; set; } = false;
        public bool OrderFailed { get; set; } = false;
        public Guid? OrderId { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (_cart.Lines.Count == 0)
                ModelState.AddModelError("", "Your cart is empty");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _httpClientFactory.CreateClient("OrderAPI");

                var request = new CheckoutRequest
                {
                    CustomerId = Guid.NewGuid(), // En producción vendría del usuario autenticado
                    Items = _cart.Lines.Select(l => new CartItemDto
                    {
                        ProductId = (int)l.Product.ProductID!,
                        ProductName = l.Product.Name,
                        Quantity = l.Quantity,
                        UnitPrice = l.Product.Price
                    }).ToList()
                };

                var response = await client.PostAsJsonAsync("api/orders/checkout", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
                    OrderId = result?.OrderId;
                    OrderPlaced = true;
                    _cart.Clear();

                    _logger.LogInformation(
                        "Order placed successfully via OrderManagement.API: {OrderId}", OrderId);
                }
                else
                {
                    OrderFailed = true;
                    _logger.LogWarning("Order placement failed with status {Status}",
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                OrderFailed = true;
                _logger.LogError(ex, "Error placing order");
            }

            return Page();
        }
    }

    public class CheckoutResult
    {
        public Guid OrderId { get; set; }
    }
}