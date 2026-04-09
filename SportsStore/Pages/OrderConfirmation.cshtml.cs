using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportsStore.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        public Guid OrderId { get; set; }

        public void OnGet(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
