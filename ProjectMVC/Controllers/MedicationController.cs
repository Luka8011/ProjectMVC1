using ClassLibrary1.Data;            // <-- namespace of your DbContext (adjust if different)
using ClassLibrary1.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Controllers
{
    public class MedicationController : Controller
    {
        // Rename HospitalContext to your actual DbContext class.
        private readonly HospitalDbContext _context;

        public MedicationController(HospitalDbContext context)
        {
            _context = context;
        }

        private const int PageSize = 9;

        // GET: /Medication?searchString=para&inStockOnly=true&page=2
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, bool inStockOnly = false, int page = 1)
        {
            var query = _context.Medications.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(m => m.MedicationName != null &&
                                         m.MedicationName.Contains(searchString));
            }

            if (inStockOnly)
            {
                query = query.Where(m => m.StockQuantity > 0);
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);

            var medications = await query
                .OrderBy(m => m.MedicationName)
                .ThenBy(m => m.MedicationId)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            ViewData["InStockOnly"] = inStockOnly;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalCount"] = totalCount;
            ViewData["PageSize"] = PageSize;

            return View(medications);
        }

        // GET: /Medication/Prescription?medicationId=5
        [HttpGet]
        public async Task<IActionResult> Prescription(int medicationId)
        {
            if (!IsDoctor())
                return RedirectToAction("Login", "Account");

            var medication = await _context.Medications.FindAsync(medicationId);
            if (medication == null)
                return NotFound();

            ViewBag.Patients = await _context.Patients
                .AsNoTracking()
                .OrderBy(p => p.PatientLastName)
                .ThenBy(p => p.PatientFirstName)
                .ToListAsync();

            return View(medication);
        }

        // POST: /Medication/Prescribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prescribe(int medicationId, int patientId,
                                                   string dosage, string frequency, int durationDays)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!IsDoctor() || userId == null)
                return RedirectToAction("Login", "Account");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FkUserId == userId);
            var patient = await _context.Patients.FindAsync(patientId);
            var medication = await _context.Medications.FindAsync(medicationId);

            if (doctor == null || patient == null || medication == null
                || string.IsNullOrWhiteSpace(dosage)
                || string.IsNullOrWhiteSpace(frequency)
                || durationDays < 1)
            {
                TempData["Error"] = "Please select a patient and fill in dosage, frequency and duration.";
                return RedirectToAction(nameof(Prescription), new { medicationId });
            }

            _context.Prescriptions.Add(new Prescription
            {
                PrescriptionDate = DateTime.Now,
                Dosage = dosage.Trim(),
                Frequency = frequency.Trim(),
                DurationDays = durationDays,
                FkPatientId = patient.PatientId,
                FkDoctorId = doctor.DoctorId,
                FkMedicationId = medication.MedicationId
            });
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"{medication.MedicationName} prescribed to {patient.PatientFirstName} {patient.PatientLastName}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Medication/MyPrescriptions  (patient sees their own prescriptions)
        [HttpGet]
        public async Task<IActionResult> MyPrescriptions()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.FkUserId == userId);

            if (patient == null)
            {
                TempData["Error"] = "No patient profile is linked to your account.";
                return RedirectToAction(nameof(Index));
            }

            var prescriptions = await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.FkMedication)
                .Include(p => p.FkDoctor)
                .Where(p => p.FkPatientId == patient.PatientId)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            return View(prescriptions);
        }

        private bool IsDoctor() =>
            HttpContext.Session.GetString("UserRole") == "Doctor";
    }
}
