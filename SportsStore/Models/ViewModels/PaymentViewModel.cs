namespace SportsStore.Models.ViewModels
{
    public class PaymentViewModel
    {
        public Order Order { get; set; } = new();
        public string ClientSecret { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
    }
}