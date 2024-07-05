using AirportApplication.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace AirportApplication.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Display(Name = "User Email")]
        public string? AirportApplicationUserId { get; set; }
        [Display(Name = "User Email")] 
        public AirportApplicationUser? AirportApplicationUser { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}
