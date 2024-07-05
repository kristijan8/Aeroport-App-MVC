using AirportApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirportApplication.ViewModels
{
    public class FlightFilter
    {
        public IList<Flight> Flights { get; set; }

        public SelectList Origins { get; set; }

        public string Origin { get; set; }

        public SelectList Destinations { get; set; }

        public string Destination { get; set; }
    }
}
