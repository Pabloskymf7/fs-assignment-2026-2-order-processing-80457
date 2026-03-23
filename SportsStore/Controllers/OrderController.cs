using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        private readonly ILogger<OrderController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly string _publishableKey;

        public OrderController(IOrderRepository repoService, Cart cartService,
            ILogger<OrderController> logger, IPaymentService paymentService,
            IOptions<SportsStore.Models.StripeSettings> stripeSettings)
        {
            repository = repoService;
            cart = cartService;
            _logger = logger;
            _paymentService = paymentService;
            _publishableKey = stripeSettings.Value.PublishableKey;
        }

        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            try
            {
                if (cart.Lines.Count() == 0)
                {
                    _logger.LogWarning("Checkout attempted with empty cart");
                    ModelState.AddModelError("", "Sorry, your cart is empty!");
                }
                if (ModelState.IsValid)
                {
                    var intent = _paymentService.CreatePaymentIntent(cart);
                    order.PaymentIntentId = intent.Id;
                    order.PaymentStatus = "Pending";
                    if (TempData != null)
                    {
                        TempData["OrderData"] = System.Text.Json.JsonSerializer.Serialize(order);
                    }
                    _logger.LogInformation("Payment intent created: {PaymentIntentId}", intent.Id);
                    return View("Payment", new PaymentViewModel
                    {
                        Order = order,
                        ClientSecret = intent.ClientSecret,
                        PublishableKey = _publishableKey
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order for {CustomerName}", order.Name);
                throw;
            }
            return View(order);
        }

        [HttpGet]
        public IActionResult ConfirmPayment(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var intent = service.Get(paymentIntentId);
                if (intent.Status == "succeeded")
                {
                    var orderJson = TempData?["OrderData"]?.ToString();
                    var order = System.Text.Json.JsonSerializer.Deserialize<Order>(orderJson ?? "");
                    if (order != null)
                    {
                        order.Lines = cart.Lines.ToArray();
                        order.PaymentStatus = "Paid";
                        repository.SaveOrder(order);
                        cart.Clear();
                        _logger.LogInformation("Order created: {OrderId} for {CustomerName}, Payment: {PaymentIntentId}",
                            order.OrderID, order.Name, paymentIntentId);
                        foreach (var line in order.Lines)
                        {
                            _logger.LogInformation("Order {OrderId} contains product: {ProductId} {ProductName} Quantity: {Quantity} Price: {Price}",
                                order.OrderID, line.Product.ProductID, line.Product.Name, line.Quantity, line.Product.Price);
                        }
                        return RedirectToPage("/Completed", new { orderId = order.OrderID });
                    }
                }
                else if (intent.Status == "canceled")
                {
                    _logger.LogWarning("Payment cancelled for intent {PaymentIntentId}", paymentIntentId);
                    return RedirectToAction("PaymentCancelled");
                }
                else
                {
                    _logger.LogWarning("Payment failed for intent {PaymentIntentId}", paymentIntentId);
                    return RedirectToAction("PaymentFailed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment {PaymentIntentId}", paymentIntentId);
                throw;
            }
            return RedirectToAction("PaymentFailed");
        }

        public ViewResult PaymentFailed() => View();
        public ViewResult PaymentCancelled() => View();
    }
}