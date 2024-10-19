using System;
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
    public class TransactionsController : Controller
    {
        private readonly InventoryContext _context;

        public TransactionsController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {        

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Query transactions for the logged-in user
            var transactionsQuery = _context.Transactions
                .Include(t => t.Products)
                .Include(t => t.Suppliers)
                .Where(t => t.UserId == userId);

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.TransactionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Set the endDate time to 23:59:59 to include the whole day
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                transactionsQuery = transactionsQuery.Where(t => t.TransactionDate <= endOfDay);
            }

            // Get the filtered transactions
            var transactions = await transactionsQuery.ToListAsync();

            // Pass the start and end dates to the view for form population
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(transactions);
        }


        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve UserId from session
            var transaction = await _context.Transactions
                .Include(t => t.Products)
                .Include(t => t.Suppliers)
                .FirstOrDefaultAsync(m => m.TransactionId == id && m.UserId == userId); // Check user ownership
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            var transaction = new Transactions
            {
                TransactionDate = DateTime.Now // Set default date and time
            };

            var userId = (int)HttpContext.Session.GetInt32("UserId");

            ViewData["ProductsId"] = new SelectList(
                _context.Products.Where(p => p.UserId == userId), // Assuming Product has UserId
                "ProductsId",
                "Name"
            );

            ViewData["SuppliersId"] = new SelectList(
                _context.Suppliers.Where(s => s.UserId == userId), // Assuming Supplier has UserId
                "SuppliersId",
                "Name"
            );
            return View(transaction);
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,ProductsId,SuppliersId,TransactionType,QuantityChanged,TransactionDate")] Transactions transaction)
        {
            //if (ModelState.IsValid)
            //{
                transaction.TransactionDate = transaction.TransactionDate == default ? DateTime.Now : transaction.TransactionDate;

                // Get UserId from session and associate it with the transaction              
                transaction.UserId = (int)HttpContext.Session.GetInt32("UserId");

                _context.Add(transaction);
                await _context.SaveChangesAsync();

                var product = await _context.Products.FindAsync(transaction.ProductsId);
                if (product != null)
                {
                    if (transaction.TransactionType == "IN")
                    {
                        product.Quantity += transaction.QuantityChanged;
                    }
                    else
                    {
                        product.Quantity -= transaction.QuantityChanged;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            //}

            // If we reach this point, something failed, redisplay form
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "Name", transaction.ProductsId);
            ViewData["SuppliersId"] = new SelectList(_context.Suppliers, "SuppliersId", "Name", transaction.SuppliersId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.UserId != HttpContext.Session.GetInt32("UserId")) // Check user ownership
            {
                return NotFound();
            }

            var userId = (int)HttpContext.Session.GetInt32("UserId");

            ViewData["ProductsId"] = new SelectList(
                _context.Products.Where(p => p.UserId == userId), // Assuming Product has UserId
                "ProductsId",
                "Name"
            );

            ViewData["SuppliersId"] = new SelectList(
                _context.Suppliers.Where(s => s.UserId == userId), // Assuming Supplier has UserId
                "SuppliersId",
                "Name"
            );
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,ProductsId,SuppliersId,TransactionType,QuantityChanged,TransactionDate")] Transactions transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail); ;
            if (userEmail == null)
            {
                // If the session doesn't contain the UserId, redirect to login
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Check if the user owns this transaction
                //Console.WriteLine("****");
                //Console.WriteLine(transaction.UserId);
                //Console.WriteLine(user.UserId);
                //if (transaction.UserId != user.UserId)
                //{
                //    // Redirect to an "Access Denied" or error page instead of simply forbidding
                //    return RedirectToAction("AccessDenied", "Account");
                //}

                // If no transaction date is provided, set the current date
                if(user != null)
                {
                    transaction.TransactionDate = transaction.TransactionDate == default ? DateTime.Now : transaction.TransactionDate;
                    transaction.UserId = user.UserId;
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionsExists(transaction.TransactionId))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Index));

            // Optionally, handle invalid model state if needed
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "Name", transaction.ProductsId);
            ViewData["SuppliersId"] = new SelectList(_context.Suppliers, "SuppliersId", "Name", transaction.SuppliersId);
            return View(transaction);
        }


        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Products)
                .Include(t => t.Suppliers)
                .FirstOrDefaultAsync(m => m.TransactionId == id && m.UserId == HttpContext.Session.GetInt32("UserId")); // Check user ownership
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null && transaction.UserId == HttpContext.Session.GetInt32("UserId")) // Check user ownership
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TransactionsExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
