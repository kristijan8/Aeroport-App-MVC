using AirportApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirportApplication.ViewModels
{
    public class PilotFilter
    {
        public IList<Pilot> Pilots { get; set; }

        public string FullName { get; set; }

        public SelectList Ranks { get; set; }

        public string Rank { get; set; }

        public SelectList Companies { get; set; }

        public string Company { get; set; }
       }
}
