using Azure;
using ClassLibrary1.Data;
using ClassLibrary1.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(HospitalDbContext context, JwtTokenService jwtTokenService, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = passwordHasher;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string UserEmail, string UserPassword)
        {
            // Query by email only (UserPassword in DB is hashed, direct string match fails)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == UserEmail);

            if (user == null || string.IsNullOrEmpty(user.UserPassword))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.UserPassword, UserPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            // Set Session values
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName ?? "");
            HttpContext.Session.SetString("UserRole", user.UserRole ?? "");

            var token = _jwtTokenService.GenerateToken(user);

            Response.Cookies.Append("JwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string UserName, string UserEmail, string UserPassword)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.UserEmail == UserEmail);

            if (existingUser)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return View();
            }

            var newUser = new User
            {
                UserName = UserName,
                UserEmail = UserEmail,
                UserRole = "User"
            };
            newUser.UserPassword = _passwordHasher.HashPassword(newUser, UserPassword);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Set Session values for the newly registered user
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("UserName", newUser.UserName ?? "");
            HttpContext.Session.SetString("UserRole", newUser.UserRole ?? "");

            var token = _jwtTokenService.GenerateToken(newUser);

            Response.Cookies.Append("JwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Clear session and JWT cookie
            HttpContext.Session.Clear();
            Response.Cookies.Delete("JwtToken");

            return RedirectToAction("Login");
        }
    }
}