using Microsoft.AspNetCore.Mvc;

namespace ProjectMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userName = HttpContext.Session.GetString("UserName");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.WelcomeMessage = $"Welcome back, {userName}!";
            return View();
        }
    }
}