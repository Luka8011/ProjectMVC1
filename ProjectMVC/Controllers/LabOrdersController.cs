using ClassLibrary1.Data;
using ClassLibrary1.Data.Models;
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userRole != "Doctor")
            {
                return Forbid();
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FkUserId == userId);
            if (doctor == null)
            {
                return NotFound("Doctor record not found.");
            }

            ViewBag.DoctorId = doctor.DoctorId;
            ViewBag.Patients = await _context.Patients
                .Select(p => new { p.PatientId, Name = $"{p.PatientFirstName} {p.PatientLastName}" })
                .ToListAsync();

            ViewBag.LabTests = await _context.LabTests.ToListAsync();

            return View("Views/Labs/CreateLabOrder.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int patientId, List<int> selectedTestIds)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null || userRole != "Doctor")
            {
                return RedirectToAction("Login", "Account");
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FkUserId == userId);
            if (doctor == null)
            {
                return NotFound();
            }

            if (patientId <= 0 || selectedTestIds == null || !selectedTestIds.Any())
            {
                ModelState.AddModelError("", "Please select a patient and at least one lab test.");

                ViewBag.DoctorId = doctor.DoctorId;
                ViewBag.Patients = await _context.Patients
                    .Select(p => new { p.PatientId, Name = $"{p.PatientFirstName} {p.PatientLastName}" })
                    .ToListAsync();
                ViewBag.LabTests = await _context.LabTests.ToListAsync();

                return View("Views/Labs/CreateLabOrder.cshtml");
            }

            var labOrder = new LabOrder
            {
                FkPatientId = patientId,
                FkDoctorId = doctor.DoctorId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                LabResults = null
            };

            _context.LabOrders.Add(labOrder);
            await _context.SaveChangesAsync();

            foreach (var testId in selectedTestIds)
            {
                var labResult = new LabResult
                {
                    FkLabOrderId = labOrder.LabOrderId,
                    FkLabTestId = testId,
                    ResultValue = "Pending"
                };
                _context.LabResults.Add(labResult);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}