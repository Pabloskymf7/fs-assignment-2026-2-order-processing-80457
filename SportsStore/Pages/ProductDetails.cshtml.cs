using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Models;

namespace SportsStore.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly IStoreRepository _repository;
        private readonly Cart _cart;
        private readonly ILogger<ProductDetailsModel> _logger;

        public ProductDetailsModel(IStoreRepository repository, Cart cart,
            ILogger<ProductDetailsModel> logger)
        {
            _repository = repository;
            _cart = cart;
            _logger = logger;
        }

        public Product? Product { get; set; }

        public void OnGet(long productId)
        {
            Product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (Product == null)
                _logger.LogWarning("Product not found: {ProductId}", productId);
        }

        public IActionResult OnPostAddToCart(long productId, int quantity, string returnUrl)
        {
            var product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                _cart.AddItem(product, quantity);
                _logger.LogInformation("Product added to cart: {ProductId} {ProductName} Quantity: {Quantity} {EventType}",
                    product.ProductID, product.Name, quantity, "AddToCart");
            }
            return RedirectToPage("/Cart", new { returnUrl });
        }
    }
}
