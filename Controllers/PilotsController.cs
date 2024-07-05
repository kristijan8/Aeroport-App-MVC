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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace AirportApplication.Controllers
{
    public class PilotsController : Controller
    {
        private readonly AirportApplicationContext _context;

        public PilotsController(AirportApplicationContext context)
        {
            _context = context;
        }

        // GET: Pilots
        public async Task<IActionResult> Index(string FullName, string Rank, string Company)
        {
            IQueryable<Pilot> pilotsQuery = _context.Pilot.AsQueryable().Include(x => x.Company);
            IQueryable<string> ranksQuery = _context.Pilot.OrderBy(x => x.Rank).Select(x => x.Rank).Distinct();
            IQueryable<string> companyQuery = _context.Pilot.OrderBy(x => x.Company.Title).Select(x => x.Company.Title).Distinct();

            if (!string.IsNullOrEmpty(FullName))
            {
                if (FullName.Contains(" "))
                {
                    string[] names = FullName.Split(" ");
                    pilotsQuery = pilotsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    pilotsQuery = pilotsQuery.Where(x => x.FirstName.Contains(FullName) || x.LastName.Contains(FullName));
                }
            }
            if (!string.IsNullOrEmpty(Rank))
            {
                pilotsQuery = pilotsQuery.Where(x => x.Rank.Contains(Rank));
            }
            if (!string.IsNullOrEmpty(Company))
            {
                pilotsQuery = pilotsQuery.Where(x => x.Company.Title.Contains(Company));
            }

            var PilotFilterVM = new PilotFilter
            {
                Pilots = await pilotsQuery.Include(x => x.Company).ToListAsync(),
                Ranks = new SelectList(await ranksQuery.ToListAsync()),
                Companies = new SelectList(await companyQuery.ToListAsync())
            };

            return View(PilotFilterVM);
        }

        // GET: Pilots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pilot == null)
            {
                return NotFound();
            }

            var pilot = await _context.Pilot
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pilot == null)
            {
                return NotFound();
            }

            return View(pilot);
        }

        // GET: Pilots/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title");
            return View();
        }

        // POST: Pilots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditPilot viewmodel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (viewmodel.ProfilePictureFile != null)
                {
                    uniqueFileName = UploadedFile(viewmodel);
                }
                Pilot pilot = new Pilot
                {
                    FirstName = viewmodel.Pilot.FirstName,
                    LastName = viewmodel.Pilot.LastName,
                    Rank = viewmodel.Pilot.Rank,
                    DateOfBirth = viewmodel.Pilot.DateOfBirth,
                    Nationality = viewmodel.Pilot.Nationality,
                    ProfilePicture = uniqueFileName,
                    CompanyId = viewmodel.Pilot.CompanyId
                };
                _context.Add(pilot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title", viewmodel.Pilot.CompanyId);
            return View(viewmodel);
        }

        // GET: Pilots/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pilot == null)
            {
                return NotFound();
            }

            var pilot = await _context.Pilot.FindAsync(id);
            if (pilot == null)
            {
                return NotFound();
            }

            AddOrEditPilot viewmodel = new AddOrEditPilot
            {
                Pilot = pilot,
                ProfilePictureName = pilot.ProfilePicture
            };

            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title", pilot.CompanyId);
            return View(viewmodel);
        }

        // POST: Pilots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddOrEditPilot viewmodel)
        {
            if (id != viewmodel.Pilot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfilePictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Pilot.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Pilot.ProfilePicture = viewmodel.ProfilePictureName;
                    }
                    _context.Update(viewmodel.Pilot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PilotExists(viewmodel.Pilot.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Title", viewmodel.Pilot.CompanyId);
            return View(viewmodel);
        }

        private string UploadedFile(AddOrEditPilot viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.ProfilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }

        // GET: Pilots/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pilot == null)
            {
                return NotFound();
            }

            var pilot = await _context.Pilot
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pilot == null)
            {
                return NotFound();
            }

            return View(pilot);
        }

        // POST: Pilots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pilot == null)
            {
                return Problem("Entity set 'AirportApplicationContext.Pilot'  is null.");
            }
            var pilot = await _context.Pilot.FindAsync(id);
            if (pilot != null)
            {
                _context.Pilot.Remove(pilot);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PilotExists(int id)
        {
          return (_context.Pilot?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: PilotsByCompany
        public async Task<IActionResult> PilotsByCompany(int? Id, string FullName, string Rank, string Company)
        {
            IQueryable<Pilot> pilotsQuery = _context.Pilot.Where(x => x.CompanyId == Id).Include(x => x.Company);
            IQueryable<string> ranksQuery = _context.Pilot.OrderBy(x => x.Rank).Select(x => x.Rank).Distinct();
            IQueryable<string> companyQuery = _context.Pilot.OrderBy(x => x.Company.Title).Select(x => x.Company.Title).Distinct();

            var company = await _context.Company.FirstOrDefaultAsync(m => m.Id == Id);
            ViewBag.Message = company.Title;

            if (!string.IsNullOrEmpty(FullName))
            {
                if (FullName.Contains(" "))
                {
                    string[] names = FullName.Split(" ");
                    pilotsQuery = pilotsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    pilotsQuery = pilotsQuery.Where(x => x.FirstName.Contains(FullName) || x.LastName.Contains(FullName));
                }
            }
            if (!string.IsNullOrEmpty(Rank))
            {
                pilotsQuery = pilotsQuery.Where(x => x.Rank.Contains(Rank));
            }
            if (!string.IsNullOrEmpty(Company))
            {
                pilotsQuery = pilotsQuery.Where(x => x.Company.Title.Contains(Company));
            }

            var PilotFilterVM = new PilotFilter
            {
                Pilots = await pilotsQuery.Include(x => x.Company).ToListAsync(),
                Ranks = new SelectList(await ranksQuery.ToListAsync()),
                Companies = new SelectList(await companyQuery.ToListAsync())
            };

            return View(PilotFilterVM);
        }
    }
}
