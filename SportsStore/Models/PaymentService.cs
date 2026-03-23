using Stripe;
namespace SportsStore.Models
{
    public class PaymentService : IPaymentService
    {
        public PaymentIntent CreatePaymentIntent(Cart cart)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(cart.Lines.Sum(l => l.Product.Price * l.Quantity) * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            return service.Create(options);
        }
    }
}