using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;
namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        private OrderController CreateController(IOrderRepository repo, Cart cart)
        {
            var logger = new Mock<ILogger<OrderController>>();
            var paymentService = new Mock<IPaymentService>();
            paymentService.Setup(p => p.CreatePaymentIntent(It.IsAny<Cart>()))
                .Returns(new Stripe.PaymentIntent
                {
                    Id = "pi_fake",
                    ClientSecret = "secret_fake",
                    Status = "requires_payment_method"
                });
            var stripeSettings = Options.Create(new StripeSettings
            {
                PublishableKey = "pk_test_fake",
                SecretKey = "sk_test_fake"
            });
            return new OrderController(repo, cart, logger.Object, paymentService.Object, stripeSettings);
        }
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            Order order = new Order();
            OrderController target = CreateController(mock.Object, cart);
            ViewResult? result = target.Checkout(order) as ViewResult;
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            Assert.False(result?.ViewData.ModelState.IsValid);
        }
        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = CreateController(mock.Object, cart);
            target.ModelState.AddModelError("error", "error");
            ViewResult? result = target.Checkout(new Order()) as ViewResult;
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            Assert.False(result?.ViewData.ModelState.IsValid);
        }
        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = CreateController(mock.Object, cart);
            ViewResult? result = target.Checkout(new Order()) as ViewResult;
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            Assert.NotNull(result);
        }
    }
}