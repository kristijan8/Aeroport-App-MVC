using System.ComponentModel.DataAnnotations;

namespace AirportApplication.Models
{
    public class Company
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Number of airplanes")]
        public int NumberOfAirplanes { get; set; }

        public string? Logo { get; set; }

        [Display(Name = "Pilots")]
        public ICollection<Pilot>? Pilots { get; set; }

        public ICollection<CompanyFlight>? CompanyFlights { get; set; }
    }
}
