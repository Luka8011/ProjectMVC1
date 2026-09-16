using ClassLibrary1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Controllers
{
    public class LabResultsController : Controller
    {
        private readonly HospitalDbContext _context;

        public LabResultsController(HospitalDbContext context)
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

            var query = _context.LabResults
                .Include(r => r.FkLabTest)
                .Include(r => r.FkLabOrder)
                    .ThenInclude(o => o.FkDoctor)
                .Include(r => r.FkLabOrder)
                    .ThenInclude(o => o.FkPatient)
                .AsNoTracking();

            if (userRole == "Doctor")
            {
                query = query.Where(r => r.FkLabOrder != null &&
                                         r.FkLabOrder.FkDoctor != null &&
                                         r.FkLabOrder.FkDoctor.FkUserId == userId);
            }
            else
            {
                query = query.Where(r => r.FkLabOrder != null &&
                                         r.FkLabOrder.FkPatient != null &&
                                         r.FkLabOrder.FkPatient.FkUserId == userId);
            }

            var results = await query.OrderByDescending(r => r.ResultDate).ToListAsync();

            return View("Views/Labs/LabResults.cshtml", results);
        }
    }
}