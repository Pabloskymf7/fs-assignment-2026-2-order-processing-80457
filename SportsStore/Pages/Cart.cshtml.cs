using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Infrastructure;
using SportsStore.Models;
namespace SportsStore.Pages {
    public class CartModel : PageModel {
        private IStoreRepository repository;
        private readonly ILogger<CartModel> _logger;
        public CartModel(IStoreRepository repo, Cart cartService, ILogger<CartModel> logger) {
            repository = repo;
            Cart = cartService;
            _logger = logger;
        }
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; } = "/";
        public void OnGet(string returnUrl) {
            ReturnUrl = returnUrl ?? "/";
        }
        public IActionResult OnPost(long productId, string returnUrl) {
            Product? product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);
            if (product != null) {
                Cart.AddItem(product, 1);
                _logger.LogInformation("Product added to cart: {ProductId} {ProductName} Price: {Price}",
                    product.ProductID, product.Name, product.Price);
            } else {
                _logger.LogWarning("Attempted to add non-existent product: {ProductId}", productId);
            }
            return RedirectToPage(new { returnUrl = returnUrl });
        }
        public IActionResult OnPostRemove(long productId, string returnUrl) {
            var product = Cart.Lines.First(cl => cl.Product.ProductID == productId).Product;
            Cart.RemoveLine(product);
            _logger.LogInformation("Product removed from cart: {ProductId} {ProductName}",
                product.ProductID, product.Name);
            return RedirectToPage(new { returnUrl = returnUrl });
        }
    }
}