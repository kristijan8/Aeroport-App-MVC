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
    public class CompaniesController : Controller
    {
        private readonly AirportApplicationContext _context;

        public CompaniesController(AirportApplicationContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index(string Title, string Country)
        {
            IQueryable<Company> companyQuery = _context.Company.AsQueryable().Include(x => x.Pilots);
            IQueryable<string> countriesQuery = _context.Company.OrderBy(x => x.Country).Select(x => x.Country).Distinct();

            if (!string.IsNullOrEmpty(Title))
            {
                companyQuery = companyQuery.Where(x => x.Title.Contains(Title));
            }
            if (!string.IsNullOrEmpty(Country))
            {
                companyQuery = companyQuery.Where(x => x.Country.Contains(Country));
            }

            var CompanyFilterVM = new CompanyFilter
            {
                Companies = await companyQuery.Include(x => x.Pilots).ToListAsync(),
                Countries = new SelectList(await countriesQuery.ToListAsync())
            };

            return View(CompanyFilterVM);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditCompany viewmodel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (viewmodel.LogoFile != null)
                {
                    uniqueFileName = UploadedFile(viewmodel);
                }
                Company company = new Company
                {
                    Title = viewmodel.Company.Title,
                    Country = viewmodel.Company.Country,
                    NumberOfAirplanes = viewmodel.Company.NumberOfAirplanes,
                    Logo = uniqueFileName,
                    Pilots = viewmodel.Company.Pilots
                };
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            AddOrEditCompany viewmodel = new AddOrEditCompany
            {
                Company = company,
                LogoName = company.Logo
            };

            return View(viewmodel);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddOrEditCompany viewmodel)
        {
            if (id != viewmodel.Company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.LogoFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Company.Logo = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Company.Logo = viewmodel.LogoName;
                    }

                    _context.Update(viewmodel.Company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(viewmodel.Company.Id))
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
            return View(viewmodel);
        }

        private string UploadedFile(AddOrEditCompany viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.LogoFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.LogoFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.LogoFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Company == null)
            {
                return Problem("Entity set 'AirportApplicationContext.Company'  is null.");
            }
            var company = await _context.Company.FindAsync(id);

            //If we delete the company, we want the pilots working place to be set to null
            //so we can remove the company (cascade delete)
            IQueryable<Pilot> pilots = _context.Pilot.Where(x => x.CompanyId == id);
            foreach (var pilot in pilots)
            {
                pilot.CompanyId = null;
            }

            if (company != null)
            {
                _context.Company.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Company?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
