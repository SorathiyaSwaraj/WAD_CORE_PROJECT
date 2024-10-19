using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using wad_core_project.Models;

namespace wad_core_project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InventoryContext _context;  // Add context to interact with the database

        public HomeController(ILogger<HomeController> logger, InventoryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Retrieve the user email from the session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(); // Handle cases where the session might be empty
            }

            // Fetch the userId using the email (assuming User model has email stored)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound(); // Handle cases where the user is not found in the database
            }

            // Fetch products where quantity is low and belong to the logged-in user
            var lowQuantityProducts = await _context.Products
                .Where(p => p.Quantity <= 10 && p.UserId == user.UserId) // Assuming Product has a UserId field
                .ToListAsync();

            return View(lowQuantityProducts); // Pass the low quantity products to the view
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
