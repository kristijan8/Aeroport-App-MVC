using AirportApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace AirportApplication.ViewModels
{
    public class AddOrEditCompany
    {
        public Company? Company { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? LogoFile { get; set; }

        [Display(Name = "Logo")]
        public string? LogoName { get; set; }
    }
}
