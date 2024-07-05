using AirportApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirportApplication.ViewModels
{
    public class CompanyFlightFilter
    {
        public IList<CompanyFlight> CompanyFlights { get; set; }

        public string Title { get; set; }

        public SelectList Origins { get; set; }

        public string Origin { get; set; }

        public SelectList Destinations { get; set; }

        public string Destination { get; set; }

        public SelectList SortTypes { get; set; }

        public string Sort { get; set; }
    }
}
