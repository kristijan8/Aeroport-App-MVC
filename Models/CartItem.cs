using System.ComponentModel.DataAnnotations;

namespace AirportApplication.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Display(Name = "Cart")]
        public int? CartId { get; set; }
        [Display(Name = "Cart")]
        public Cart? Cart { get; set; }

        [Display(Name = "Flight")]
        public long? CompanyFlightId { get; set; }
        [Display(Name = "Flight")]
        public CompanyFlight? CompanyFlight { get; set; }

        public int Quantity { get; set; }

        [Display(Name = "Total Price")]
        public decimal getTotalPricePerFlight
        {
            get
            {
                return Quantity * CompanyFlight.Price;
            }
        }
    }
}
