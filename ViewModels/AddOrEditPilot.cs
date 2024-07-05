using AirportApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace AirportApplication.ViewModels
{
    public class AddOrEditPilot
    {
        public Pilot? Pilot { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? ProfilePictureName { get; set; }
    }
}
