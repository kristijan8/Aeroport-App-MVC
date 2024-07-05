using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportApplication.Data;
using AirportApplication.Models;
using AirportApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AirportApplication.Controllers
{
    public class CompanyFlightsController : Controller
    {
        private readonly AirportApplicationContext _context;

        public CompanyFlightsController(AirportApplicationContext context)
        {
            _context = context;
        }

        // GET: CompanyFlights
        public async Task<IActionResult> Index(string Title, string Origin, string Destination, string Sort)
        {
            IQueryable<CompanyFlight> companyFlightQuery = _context.CompanyFlight.AsQueryable().Include(x => x.Flight).Include(x => x.Companies);
            IQueryable<string> originQuery = _context.CompanyFlight.OrderBy(x => x.Flight.Origin).Select(x => x.Flight.Origin).Distinct();
            IQueryable<string> destinationQuery = _context.CompanyFlight.OrderBy(x => x.Flight.Destination).Select(x => x.Flight.Destination).Distinct();
            List<string> sortTypesList = new List<string>(2);
            sortTypesList.Add("Ascending");
            sortTypesList.Add("Descending");
            

            if (!string.IsNullOrEmpty(Title))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Companies.Title.Contains(Title));
            }
            if (!string.IsNullOrEmpty(Origin))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Flight.Origin.Contains(Origin));
            }
            if (!string.IsNullOrEmpty(Destination))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Flight.Destination.Contains(Destination));
            }
            if (!string.IsNullOrEmpty(Sort))
            {
                if (string.Compare(Sort, "Ascending") == 0)
                {
                    companyFlightQuery = companyFlightQuery.OrderBy(x => x.Price);
                }
                else
                {
                    companyFlightQuery = companyFlightQuery.OrderByDescending(x => x.Price);
                }
            }

            var CompanyFlightFilterVM = new CompanyFlightFilter
            {
                CompanyFlights = await companyFlightQuery.ToListAsync(),
                Origins = new SelectList(await originQuery.ToListAsync()),
                Destinations = new SelectList(await destinationQuery.ToListAsync()),
                SortTypes = new SelectList (sortTypesList)
            };

            return View(CompanyFlightFilterVM);
        }

        // GET: CompanyFlights/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.CompanyFlight == null)
            {
                return NotFound();
            }

            var companyFlight = await _context.CompanyFlight
                .Include(c => c.Companies)
                .Include(c => c.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyFlight == null)
            {
                return NotFound();
            }

            return View(companyFlight);
        }

        // GET: CompanyFlights/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title");
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "FlightName");
            return View();
        }

        // POST: CompanyFlights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,FlightId,Price,DateOfFlight,Duration")] CompanyFlight companyFlight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyFlight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title", companyFlight.CompanyId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "FlightName", companyFlight.FlightId);
            return View(companyFlight);
        }

        // GET: CompanyFlights/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.CompanyFlight == null)
            {
                return NotFound();
            }

            var companyFlight = await _context.CompanyFlight.FindAsync(id);
            if (companyFlight == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title", companyFlight.CompanyId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "FlightName", companyFlight.FlightId);
            return View(companyFlight);
        }

        // POST: CompanyFlights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, [Bind("Id,CompanyId,FlightId,Price,DateOfFlight,Duration")] CompanyFlight companyFlight)
        {
            if (id != companyFlight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyFlight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyFlightExists(companyFlight.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title", companyFlight.CompanyId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "FlightName", companyFlight.FlightId);
            return View(companyFlight);
        }

        // GET: CompanyFlights/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.CompanyFlight == null)
            {
                return NotFound();
            }

            var companyFlight = await _context.CompanyFlight
                .Include(c => c.Companies)
                .Include(c => c.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyFlight == null)
            {
                return NotFound();
            }

            return View(companyFlight);
        }

        // POST: CompanyFlights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.CompanyFlight == null)
            {
                return Problem("Entity set 'AirportApplicationContext.CompanyFlight'  is null.");
            }
            var companyFlight = await _context.CompanyFlight.FindAsync(id);
            if (companyFlight != null)
            {
                _context.CompanyFlight.Remove(companyFlight);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyFlightExists(long id)
        {
          return (_context.CompanyFlight?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: FlightsByCompany
        public async Task<IActionResult> FlightsByCompany(int? Id, string Title, string Origin, string Destination, string Sort)
        {
            IQueryable<CompanyFlight> companyFlightQuery = _context.CompanyFlight.Where(x => x.CompanyId == Id).Include(x => x.Flight).Include(x => x.Companies);
            IQueryable<string> originQuery = _context.CompanyFlight.OrderBy(x => x.Flight.Origin).Select(x => x.Flight.Origin).Distinct();
            IQueryable<string> destinationQuery = _context.CompanyFlight.OrderBy(x => x.Flight.Destination).Select(x => x.Flight.Destination).Distinct();
            List<string> sortTypesList = new List<string>(2);
            sortTypesList.Add("Ascending");
            sortTypesList.Add("Descending");

            var company = await _context.Company.FirstOrDefaultAsync(m => m.Id == Id);
            ViewBag.Message = company.Title;

            if (!string.IsNullOrEmpty(Title))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Companies.Title.Contains(Title));
            }
            if (!string.IsNullOrEmpty(Origin))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Flight.Origin.Contains(Origin));
            }
            if (!string.IsNullOrEmpty(Destination))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Flight.Destination.Contains(Destination));
            }
            if (!string.IsNullOrEmpty(Sort))
            {
                if (string.Compare(Sort, "Ascending") == 0)
                {
                    companyFlightQuery = companyFlightQuery.OrderBy(x => x.Price);
                }
                else
                {
                    companyFlightQuery = companyFlightQuery.OrderByDescending(x => x.Price);
                }
            }

            var CompanyFlightFilterVM = new CompanyFlightFilter
            {
                CompanyFlights = await companyFlightQuery.ToListAsync(),
                Origins = new SelectList(await originQuery.ToListAsync()),
                Destinations = new SelectList(await destinationQuery.ToListAsync()),
                SortTypes = new SelectList(sortTypesList)
            };

            return View(CompanyFlightFilterVM);
        }

        // GET: FlightsByDestination
        public async Task<IActionResult> FlightsByDestination(int? Id, string Title, string Origin, string Destination, string Sort)
        {
            IQueryable<CompanyFlight> companyFlightQuery = _context.CompanyFlight.Where(x => x.FlightId == Id).Include(x => x.Flight).Include(x => x.Companies);
            IQueryable<string> originQuery = _context.CompanyFlight.OrderBy(x => x.Flight.Origin).Select(x => x.Flight.Origin).Distinct();
            IQueryable<string> destinationQuery = _context.CompanyFlight.OrderBy(x => x.Flight.Destination).Select(x => x.Flight.Destination).Distinct();
            List<string> sortTypesList = new List<string>(2);
            sortTypesList.Add("Ascending");
            sortTypesList.Add("Descending");

            var flight = await _context.Flight.FirstOrDefaultAsync(m => m.Id == Id);
            ViewBag.MessageOrigin = flight.Origin;
            ViewBag.MessageDestination = flight.Destination;

            if (!string.IsNullOrEmpty(Title))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Companies.Title.Contains(Title));
            }
            if (!string.IsNullOrEmpty(Origin))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Flight.Origin.Contains(Origin));
            }
            if (!string.IsNullOrEmpty(Destination))
            {
                companyFlightQuery = companyFlightQuery.Where(x => x.Flight.Destination.Contains(Destination));
            }
            if (!string.IsNullOrEmpty(Sort))
            {
                if (string.Compare(Sort, "Ascending") == 0)
                {
                    companyFlightQuery = companyFlightQuery.OrderBy(x => x.Price);
                }
                else
                {
                    companyFlightQuery = companyFlightQuery.OrderByDescending(x => x.Price);
                }
            }

            var CompanyFlightFilterVM = new CompanyFlightFilter
            {
                CompanyFlights = await companyFlightQuery.ToListAsync(),
                Origins = new SelectList(await originQuery.ToListAsync()),
                Destinations = new SelectList(await destinationQuery.ToListAsync()),
                SortTypes = new SelectList(sortTypesList)
            };

            return View(CompanyFlightFilterVM);
        }

        // GET: CompanyFlights/BookFlight/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookFlight(long? id)
        {
            if (id == null || _context.CompanyFlight == null)
            {
                return NotFound();
            }

            var companyFlight = await _context.CompanyFlight
                .Include(c => c.Companies)
                .Include(c => c.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyFlight == null)
            {
                return NotFound();
            }

            return View(companyFlight);
        }

        // GET: CompanyFlights/BookFlightCompleted/5
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookFlightCompleted(long? id, int quantity)
        {
            if (id == null || _context.CompanyFlight == null)
            {
                return NotFound();
            }

            var companyFlight = await _context.CompanyFlight
                .Include(c => c.Companies)
                .Include(c => c.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyFlight == null)
            {
                return NotFound();
            }
            var userLoggedInId = HttpContext.Session.GetString("UserLoggedIn");
            var cart = await _context.Cart.FirstOrDefaultAsync(x => x.AirportApplicationUserId.Equals(userLoggedInId));
            if (cart == null)
            {
                return NotFound();
            }
            CartItem cartItem = new CartItem
            {
                CartId = cart.Id,
                CompanyFlightId = id,
                Quantity = quantity
            };

            _context.Add(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
