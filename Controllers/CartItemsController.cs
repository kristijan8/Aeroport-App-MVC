using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportApplication.Data;
using AirportApplication.Models;

namespace AirportApplication.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly AirportApplicationContext _context;

        public CartItemsController(AirportApplicationContext context)
        {
            _context = context;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var airportApplicationContext = _context.CartItem.Include(c => c.Cart);
            return View(await airportApplicationContext.ToListAsync());
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CartId,CompanyFlightId,Quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id", cartItem.CartId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(x => x.Cart).Include(x => x.CompanyFlight).ThenInclude(x => x.Flight).Include(x => x.CompanyFlight).ThenInclude(x => x.Companies)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id", cartItem.CartId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int quantity)
        {
            var cartItem = await _context.CartItem
                .Include(x => x.Cart).Include(x => x.CompanyFlight).ThenInclude(x => x.Flight).Include(x => x.CompanyFlight).ThenInclude(x => x.Companies)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            cartItem.Quantity = quantity;
            _context.Update(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ShowCart));
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CartItem == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(x => x.Cart).Include(x => x.CompanyFlight).ThenInclude(x => x.Flight).Include(x => x.CompanyFlight).ThenInclude(x => x.Companies)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CartItem == null)
            {
                return Problem("Entity set 'AirportApplicationContext.CartItem'  is null.");
            }
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowCart));
        }

        private bool CartItemExists(int id)
        {
          return (_context.CartItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: ShowCart
        public async Task<IActionResult> ShowCart()
        {
            var userLoggedInId = HttpContext.Session.GetString("UserLoggedIn");
            var cart = await _context.Cart.FirstOrDefaultAsync(x => x.AirportApplicationUserId.Equals(userLoggedInId));
            if (cart == null)
            {
                return NotFound();
            }
            IQueryable<CartItem> cartQuery = _context.CartItem.AsQueryable();
            cartQuery = cartQuery.Where(x => x.CartId==cart.Id);
            decimal totalPrice = 0.00M;
            List<CartItem> cartItemsList = cartQuery.ToList();
            foreach (CartItem cartItem in cartItemsList)
            {
                var companyFlight = await _context.CompanyFlight.FirstOrDefaultAsync(x => x.Id == cartItem.CompanyFlightId);
                if (companyFlight == null)
                {
                    return NotFound();
                }
                totalPrice += (cartItem.Quantity*companyFlight.Price);
            }
            string priceAsString = totalPrice.ToString("#.##");
            ViewBag.TotalPrice = priceAsString;
            return View(await cartQuery.Include(x => x.Cart).Include(x => x.CompanyFlight).ThenInclude(x => x.Flight).Include(x => x.CompanyFlight).ThenInclude(x => x.Companies).ToListAsync());
        }

        // GET: CartItems/EmptyCart
        public async Task<IActionResult> EmptyCart()
        {
            if (_context.CartItem == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST: CartItems/EmptyCart
        [HttpPost, ActionName("EmptyCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmptyCartConfirmed()
        {
            if (_context.CartItem == null)
            {
                return Problem("Entity set 'AirportApplicationContext.CartItem'  is null.");
            }

            var userLoggedInId = HttpContext.Session.GetString("UserLoggedIn");
            var cart = await _context.Cart.FirstOrDefaultAsync(x => x.AirportApplicationUserId.Equals(userLoggedInId));
            if (cart == null)
            {
                return NotFound();
            }
            IQueryable<CartItem> cartQuery = _context.CartItem.AsQueryable();
            cartQuery = cartQuery.Where(x => x.CartId == cart.Id);
            foreach (CartItem cartItem in cartQuery) 
            {
                _context.CartItem.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowCart));
        }

        // GET: CartItems/ProceedCart
        public async Task<IActionResult> ProceedCart()
        {
            if (_context.CartItem == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST: CartItems/ProceedCart
        [HttpPost, ActionName("ProceedCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProceedCartConfirmed()
        {
            if (_context.CartItem == null)
            {
                return Problem("Entity set 'AirportApplicationContext.CartItem'  is null.");
            }

            var userLoggedInId = HttpContext.Session.GetString("UserLoggedIn");
            var cart = await _context.Cart.FirstOrDefaultAsync(x => x.AirportApplicationUserId.Equals(userLoggedInId));
            if (cart == null)
            {
                return NotFound();
            }
            IQueryable<CartItem> cartQuery = _context.CartItem.AsQueryable();
            cartQuery = cartQuery.Where(x => x.CartId == cart.Id);
            foreach (CartItem cartItem in cartQuery)
            {
                _context.CartItem.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowCart));
        }
    }
}
