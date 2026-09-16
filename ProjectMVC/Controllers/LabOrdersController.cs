using ClassLibrary1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Controllers
{
    public class LabOrdersController : Controller
    {
        private readonly HospitalDbContext _context;

        public LabOrdersController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.LabOrders
                .Include(o => o.FkDoctor)
                .Include(o => o.FkPatient)
                .Include(o => o.LabResults)
                    .ThenInclude(r => r.FkLabTest)
                .AsNoTracking();

            if (userRole == "Doctor")
            {
                query = query.Where(o => o.FkDoctor != null && o.FkDoctor.FkUserId == userId);
            }
            else
            {
                query = query.Where(o => o.FkPatient != null && o.FkPatient.FkUserId == userId);
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();

            return View("Views/Labs/LabOrders.cshtml", orders);
        }
    }
}