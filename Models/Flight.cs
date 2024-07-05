using System.ComponentModel.DataAnnotations;

namespace AirportApplication.Models
{
    public class Flight
    { 
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Origin { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Destination { get; set; }

        public ICollection<CompanyFlight>? CompanyFlights { get; set; }

        [Display(Name = "Origin-Destination")]
        public string FlightName
        {
            get
            {
                return string.Format("{0}-{1}", Origin, Destination);
            }
        }
    }
}
