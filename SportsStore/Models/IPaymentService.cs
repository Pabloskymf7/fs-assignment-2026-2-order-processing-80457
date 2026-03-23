using Stripe;
namespace SportsStore.Models
{
    public interface IPaymentService
    {
        PaymentIntent CreatePaymentIntent(Cart cart);
    }
}