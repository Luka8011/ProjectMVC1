using ClassLibrary1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly HospitalDbContext _context;

        public MedicalRecordsController(HospitalDbContext context)
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

            var query = _context.MedicalRecords
                .Include(m => m.FkPatient)
                .AsNoTracking();

            if (userRole != "Doctor")
            {
                query = query.Where(m => m.FkPatient != null && m.FkPatient.FkUserId == userId);
            }

            var records = await query.OrderByDescending(m => m.RecordCreationTime).ToListAsync();

            return View("Views/Records/MedicalRecords.cshtml", records);
        }
    }
}