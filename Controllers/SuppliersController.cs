using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wad_core_project.Models;

namespace wad_core_project.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly InventoryContext _context;

        public SuppliersController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userEmail = HttpContext.Session.GetString("UserEmail");
            //var userId = _context.Users.Single(u => u.Email == userEmail).UserId;

            var suppliers = await _context.Suppliers
                .Where(s => s.UserId == userId)
                .ToListAsync();
            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SuppliersId == id);
            if (suppliers == null || suppliers.UserId != _context.Users.Single(u => u.Email == HttpContext.Session.GetString("UserEmail")).UserId)
            {
                return NotFound();
            }

            return View(suppliers);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SuppliersId,Name,Contact")] Suppliers suppliers)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user != null)
            {
                suppliers.UserId = user.UserId; // Associate the supplier with the logged-in user
                _context.Add(suppliers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suppliers);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers == null || suppliers.UserId != _context.Users.Single(u => u.Email == HttpContext.Session.GetString("UserEmail")).UserId)
            {
                return NotFound();
            }
            return View(suppliers);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SuppliersId,Name,Contact")] Suppliers suppliers)
        {
            if (id != suppliers.SuppliersId)
            {
                return NotFound();
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user != null)
            {
                suppliers.UserId = user.UserId; // Associate the supplier with the logged-in user
                try
                {
                    _context.Update(suppliers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuppliersExists(suppliers.SuppliersId))
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
            return View(suppliers);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SuppliersId == id);
            if (suppliers == null || suppliers.UserId != _context.Users.Single(u => u.Email == HttpContext.Session.GetString("UserEmail")).UserId)
            {
                return NotFound();
            }

            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers != null)
            {
                _context.Suppliers.Remove(suppliers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuppliersExists(int id)
        {
            return _context.Suppliers.Any(e => e.SuppliersId == id);
        }
    }
}
