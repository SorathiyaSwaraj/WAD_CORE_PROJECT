using Microsoft.AspNetCore.Mvc;
using wad_core_project.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace wad_core_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly InventoryContext _context;

        public AccountController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,Email,Password")] User user)
        {
            //if (ModelState.IsValid)
            //{
                // Check if the email already exists
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(user);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Hash the password
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            //}
            return View(user);
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName) // If you have a UserName property
                // Add more claims if necessary
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Set the authentication cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Set session or authentication
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetInt32("UserId", user.UserId);



            return RedirectToAction("Index", "Products");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
