using AirportApplication.Areas.Identity.Data;
using AirportApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AirportApplication.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<AirportApplicationUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            AirportApplicationUser user = await UserManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                var User = new AirportApplicationUser();
                User.Email = "admin@admin.com";
                User.UserName = "admin@admin.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            //Add User Role
            roleCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }
            user = await UserManager.FindByEmailAsync("user@user.com");
            if (user == null)
            {
                var User = new AirportApplicationUser();
                User.Email = "user@user.com";
                User.UserName = "user@user.com";
                string userPWD = "User1234";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role User
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "User"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AirportApplicationContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<AirportApplicationContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Company.Any() || context.Pilot.Any() || context.Flight.Any())
                {
                    return;
                }

                AirportApplicationUser user = context.Users.FirstOrDefault(x => x.UserName.Equals("user@user.com"));
                Cart cart = context.Cart.FirstOrDefault(x => x.AirportApplicationUserId == user.Id);
                if(cart == null)
                {
                    Cart newCart = new Cart
                    {
                        AirportApplicationUserId = user.Id
                    };
                    context.Add(newCart);
                    context.SaveChanges();
                }

                context.Company.AddRange(
                    new Company
                    {
                        Title = "Wizz Air",
                        Country = "Hungary ",
                        NumberOfAirplanes = 50,
                        Logo = "wizzair.png"
                    },
                    new Company
                    {
                        Title = "Alaska Airlines",
                        Country = "USA",
                        NumberOfAirplanes = 150,
                        Logo = "alaska.png"
                    },
                    new Company
                    {
                        Title = "Allegiant Air",
                        Country = "USA",
                        NumberOfAirplanes = 120,
                        Logo = "allegiant.png"
                    },
                    new Company
                    {
                        Title = "Turkish Airlines",
                        Country = "Turkey",
                        NumberOfAirplanes = 40,
                        Logo = "turkishAirlines.png"
                    },
                    new Company
                    {
                        Title = "EasyJet",
                        Country = "Switzerland ",
                        NumberOfAirplanes = 30,
                        Logo = "easyjet.png"
                    }
                );
                context.SaveChanges();
                
                context.Pilot.AddRange(
                    new Pilot
                    {
                        FirstName = "John",
                        LastName = "Wayne",
                        Rank = "First Officer",
                        Nationality = "American",
                        DateOfBirth = DateTime.Parse("1970-3-20"),
                        ProfilePicture = "01.jpg",
                        CompanyId = context.Company.Single(x => x.Title == "EasyJet").Id
                    },
                    new Pilot
                    {
                        FirstName = "Marko",
                        LastName = "Stojanoski",
                        Rank = "Trainee",
                        Nationality = "Macedonian",
                        DateOfBirth = DateTime.Parse("1990-6-13"),
                        ProfilePicture = "02.jpg",
                        CompanyId = context.Company.Single(x => x.Title == "Wizz Air").Id
                    },
                    new Pilot
                    {
                        FirstName = "Suleman",
                        LastName = "Ergenc",
                        Rank = "Instructor",
                        Nationality = "Turkish",
                        DateOfBirth = DateTime.Parse("1967-7-5"),
                        ProfilePicture = "03.jfif",
                        CompanyId = context.Company.Single(x => x.Title == "Turkish Airlines").Id
                    },
                    new Pilot
                    {
                        FirstName = "Paul",
                        LastName = "Oldman",
                        Rank = "Senior Flight Captain",
                        Nationality = "American",
                        DateOfBirth = DateTime.Parse("1976-8-11"),
                        ProfilePicture = "04.jfif",
                        CompanyId = context.Company.Single(x => x.Title == "Allegiant Air").Id
                    },
                    new Pilot
                    {
                        FirstName = "Ralph",
                        LastName = "Fiennes",
                        Rank = "Instructor",
                        Nationality = "German",
                        DateOfBirth = DateTime.Parse("1978-4-6"),
                        ProfilePicture = "05.jfif",
                        CompanyId = context.Company.Single(x => x.Title == "EasyJet").Id
                    },
                    new Pilot
                    {
                        FirstName = "Jean",
                        LastName = "Reno",
                        Rank = "First Officer",
                        Nationality = "French",
                        DateOfBirth = DateTime.Parse("1970-8-30"),
                        ProfilePicture = "06.jfif",
                        CompanyId = context.Company.Single(x => x.Title == "Allegiant Air").Id
                    },
                    new Pilot
                    {
                        FirstName = "Vojdan",
                        LastName = "Zdravkovic",
                        Rank = "Trainee",
                        Nationality = "Bosnian",
                        DateOfBirth = DateTime.Parse("1988-2-18"),
                        ProfilePicture = "07.jpg",
                        CompanyId = context.Company.Single(x => x.Title == "Wizz Air").Id
                    }
                );
                context.SaveChanges();

                context.Flight.AddRange(
                    new Flight
                    {
                        Origin = "Macedonia",
                        Destination = "Germany"
                    },
                    new Flight
                    {
                        Origin = "Macedonia",
                        Destination = "USA"
                    },
                    new Flight
                    {
                        Origin = "Hungary",
                        Destination = "Poland"
                    }, 
                    new Flight
                    {
                        Origin = "Italy",
                        Destination = "France"
                    },
                    new Flight
                    {
                        Origin = "France",
                        Destination = "Germany"
                    },
                    new Flight
                    {
                        Origin = "Serbia",
                        Destination = "Spain"
                    },
                    new Flight
                    {
                        Origin = "Turkey",
                        Destination = "Egypt"
                    },
                    new Flight
                    {
                        Origin = "Egypt",
                        Destination = "Sweden"
                    },
                    new Flight
                    {
                        Origin = "Finland",
                        Destination = "Russia"
                    },
                    new Flight
                    {
                        Origin = "Finland",
                        Destination = "Italy"
                    }
                );
                context.SaveChanges();

                context.CompanyFlight.AddRange(
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Turkish Airlines").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Macedonia" && d.Destination == "Germany").Id,
                        Price = 26.99M,
                        DateOfFlight = DateTime.Parse("2022-7-5 20:00:00"),
                        Duration = 180
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Allegiant Air").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Finland" && d.Destination == "Italy").Id,
                        Price = 23.99M,
                        DateOfFlight = DateTime.Parse("2022-9-21 18:45:00"),
                        Duration = 170
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Allegiant Air").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Finland" && d.Destination == "Russia").Id,
                        Price = 27.45M,
                        DateOfFlight = DateTime.Parse("2022-9-14 20:00:00"),
                        Duration = 270
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Wizz Air").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Egypt" && d.Destination == "Sweden").Id,
                        Price = 45.00M,
                        DateOfFlight = DateTime.Parse("2022-9-14 11:00:00"),
                        Duration = 300
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Turkish Airlines").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Egypt" && d.Destination == "Sweden").Id,
                        Price = 51.30M,
                        DateOfFlight = DateTime.Parse("2022-9-14 11:00:00"),
                        Duration = 300
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Allegiant Air").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Serbia" && d.Destination == "Spain").Id,
                        Price = 44.99M,
                        DateOfFlight = DateTime.Parse("2022-9-11 09:00:00"),
                        Duration = 170
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "EasyJet").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "France" && d.Destination == "Germany").Id,
                        Price = 11.99M,
                        DateOfFlight = DateTime.Parse("2022-8-17 10:30:00"),
                        Duration = 70
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Alaska Airlines").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Italy" && d.Destination == "France").Id,
                        Price = 14.50M,
                        DateOfFlight = DateTime.Parse("2022-8-11 17:20:00"),
                        Duration = 90
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "EasyJet").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Italy" && d.Destination == "France").Id,
                        Price = 18.50M,
                        DateOfFlight = DateTime.Parse("2022-8-11 17:20:00"),
                        Duration = 90
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Turkish Airlines").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Hungary" && d.Destination == "Poland").Id,
                        Price = 9.99M,
                        DateOfFlight = DateTime.Parse("2022-8-5 10:30:00"),
                        Duration = 75
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Wizz Air").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Hungary" && d.Destination == "Poland").Id,
                        Price = 10.99M,
                        DateOfFlight = DateTime.Parse("2022-8-5 10:30:00"),
                        Duration = 75
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "EasyJet").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Hungary" && d.Destination == "Poland").Id,
                        Price = 10.85M,
                        DateOfFlight = DateTime.Parse("2022-8-5 10:30:00"),
                        Duration = 75
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Wizz Air").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Macedonia" && d.Destination == "Germany").Id,
                        Price = 21.99M,
                        DateOfFlight = DateTime.Parse("2022-7-21 15:05:00"),
                        Duration = 60
                    },
                    new CompanyFlight
                    {
                        CompanyId = context.Company.Single(d => d.Title == "Alaska Airlines").Id,
                        FlightId = context.Flight.Single(d => d.Origin == "Macedonia" && d.Destination == "USA").Id,
                        Price = 70.99M,
                        DateOfFlight = DateTime.Parse("2022-7-11 05:45:00"),
                        Duration = 720
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
