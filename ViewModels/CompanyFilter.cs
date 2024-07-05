using AirportApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirportApplication.ViewModels
{
    public class CompanyFilter
    {
        public IList<Company> Companies { get; set; }

        public string Title { get; set; }

        public SelectList Countries { get; set; }

        public string Country { get; set; }
    }
}
