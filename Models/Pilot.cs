using System.ComponentModel.DataAnnotations;

namespace AirportApplication.Models
{
    public class Pilot
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string LastName { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Rank { get; set; }

        [Display(Name = "Date of birth")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(40)]
        public string? Nationality { get; set; }

        [Display(Name = "Profile Picutre")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Company")]
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
