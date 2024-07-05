using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AirportApplication.Areas.Identity.Data;

namespace AirportApplication.Data
{
    public class AirportApplicationContext : IdentityDbContext<AirportApplicationUser>
    {
        public AirportApplicationContext (DbContextOptions<AirportApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<AirportApplication.Models.Company> Company { get; set; }

        public DbSet<AirportApplication.Models.Pilot> Pilot { get; set; }

        public DbSet<AirportApplication.Models.Flight> Flight { get; set; }

        public DbSet<AirportApplication.Models.CompanyFlight> CompanyFlight { get; set; }

        public DbSet<AirportApplication.Models.Cart> Cart { get; set; }

        public DbSet<AirportApplication.Models.CartItem> CartItem { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
