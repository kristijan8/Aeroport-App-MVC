using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApplication.Models
{
    public class CompanyFlight
    {
        public long Id { get; set; }

        [Display(Name = "Company")]
        public int CompanyId { get; set; }
        [Display(Name = "Company")]
        public Company? Companies { get; set; }

        [Display(Name = "Flight")]
        public int FlightId { get; set; }
        [Display(Name = "Flight")]
        public Flight? Flight { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime DateOfFlight { get; set; }

        [Display(Name = "Duration in minutes")]
        public int Duration { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

        [Display(Name = "Date of Flight")]
        public string getDate
        {
            get
            {
                return DateOfFlight.ToShortDateString();
            }
        }

        [Display(Name = "Time of Flight")]
        public string getTime
        {
            get
            {
                return DateOfFlight.ToShortTimeString();
            }
        }
    }
}
