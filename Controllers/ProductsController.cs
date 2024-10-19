using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wad_core_project.Models;

namespace wad_core_project.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly InventoryContext _context;

        public ProductsController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Get the logged-in user's email from session or claims
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var products = await _context.Products
                .Where(p => p.UserId == _context.Users.Single(u => u.Email == userEmail).UserId)
                .ToListAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductsId == id);
            if (products == null || products.UserId != _context.Users.Single(u => u.Email == HttpContext.Session.GetString("UserEmail")).UserId)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductsId,Name,Quantity,Price,Description,Category")] Products products)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user != null)
            {
                products.UserId = user.UserId; // Associate the product with the logged-in user
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null || products.UserId != _context.Users.Single(u => u.Email == HttpContext.Session.GetString("UserEmail")).UserId)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductsId,Name,Quantity,Price,Description,Category")] Products products)
        {
            if (id != products.ProductsId)
            {
                return NotFound();
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user != null)
            {
                products.UserId = user.UserId; // Associate the product with the logged-in user
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductsId))
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
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductsId == id);
            if (products == null || products.UserId != _context.Users.Single(u => u.Email == HttpContext.Session.GetString("UserEmail")).UserId)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductsId == id);
        }
    }
}
