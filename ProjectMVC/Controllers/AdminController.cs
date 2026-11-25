using ClassLibrary1.Data;            // <-- namespace of your DbContext (adjust if different)
using ClassLibrary1.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Controllers
{
    // Admin-only: manage invoices and charges.
    // Invoice statuses used here: "Pending", "Paid", "Cancelled" (must match what your Payment controller uses).
    public class AdminController : Controller
    {
        // Rename HospitalContext to your actual DbContext class.
        private readonly HospitalDbContext _context;
        private const int PageSize = 10;

        public AdminController(HospitalDbContext context)
        {
            _context = context;
        }

        // Runs before every action in this controller: only the Admin role gets through.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                context.Result = RedirectToAction("Login", "Account");

            base.OnActionExecuting(context);
        }

        // GET: /Admin/Invoices?searchString=john&status=Pending&page=2
        [HttpGet]
        public async Task<IActionResult> Invoices(string? searchString, string? status, int page = 1)
        {
            var query = _context.Invoices
                .AsNoTracking()
                .Include(i => i.FkPatient)
                .Include(i => i.FkPrescription).ThenInclude(p => p!.FkMedication)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(i => i.InvoiceStatus == status);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(i => i.FkPatient != null &&
                    (i.FkPatient.PatientFirstName + " " + i.FkPatient.PatientLastName).Contains(searchString));
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);

            var invoices = await query
                .OrderByDescending(i => i.InvoiceCreationDate)
                .ThenByDescending(i => i.InvoiceId)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var today = DateTime.Today;
            ViewData["PendingTotal"] = await _context.Invoices
                .Where(i => i.InvoiceStatus == "Pending").SumAsync(i => i.AmountToPay ?? 0m);
            ViewData["PaidTotal"] = await _context.Invoices
                .Where(i => i.InvoiceStatus == "Paid").SumAsync(i => i.AmountToPay ?? 0m);
            ViewData["OverdueCount"] = await _context.Invoices
                .CountAsync(i => i.InvoiceStatus == "Pending" && i.DueDate < today);

            ViewData["CurrentFilter"] = searchString;
            ViewData["Status"] = string.IsNullOrWhiteSpace(status) ? "All" : status;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalCount"] = totalCount;
            ViewData["PageSize"] = PageSize;

            return View(invoices);
        }

        // GET: /Admin/CreateInvoice
        [HttpGet]
        public async Task<IActionResult> CreateInvoice()
        {
            await LoadCreateLists();
            return View();
        }

        // POST: /Admin/CreateInvoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(int patientId, int? prescriptionId,
                                                       decimal amount, DateTime dueDate)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null || amount <= 0 || amount > 1_000_000m)
            {
                TempData["Error"] = "Select a patient and enter a valid amount.";
                return RedirectToAction(nameof(CreateInvoice));
            }

            if (prescriptionId.HasValue)
            {
                var rx = await _context.Prescriptions
                    .AsNoTracking()
                    .Include(p => p.Invoices)
                    .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);

                if (rx == null || rx.FkPatientId != patient.PatientId)
                {
                    TempData["Error"] = "That prescription does not belong to the selected patient.";
                    return RedirectToAction(nameof(CreateInvoice));
                }
                if (rx.Invoices.Any())
                {
                    TempData["Error"] = "That prescription has already been invoiced.";
                    return RedirectToAction(nameof(CreateInvoice));
                }
            }

            _context.Invoices.Add(new Invoice
            {
                AmountToPay = Math.Round(amount, 2),
                InvoiceCreationDate = DateTime.Now,
                DueDate = dueDate,
                InvoiceStatus = "Pending",
                FkPatientId = patient.PatientId,
                FkUserId = patient.FkUserId,      // so it also shows up on the patient's own billing page
                FkPrescriptionId = prescriptionId
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Invoice created for {patient.PatientFirstName} {patient.PatientLastName}.";
            return RedirectToAction(nameof(Invoices));
        }

        // GET: /Admin/EditInvoice/5
        [HttpGet]
        public async Task<IActionResult> EditInvoice(int id)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.FkPatient)
                .Include(i => i.FkPrescription).ThenInclude(p => p!.FkMedication)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // POST: /Admin/EditInvoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInvoice(int id, decimal amount, DateTime dueDate, string status)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.InvoiceStatus == "Paid")
            {
                TempData["Error"] = "Paid invoices can't be changed.";
                return RedirectToAction(nameof(EditInvoice), new { id });
            }
            if (amount <= 0 || amount > 1_000_000m || (status != "Pending" && status != "Cancelled"))
            {
                TempData["Error"] = "Enter a valid amount and status.";
                return RedirectToAction(nameof(EditInvoice), new { id });
            }

            invoice.AmountToPay = Math.Round(amount, 2);
            invoice.DueDate = dueDate;
            invoice.InvoiceStatus = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Invoice #INV-{id} updated.";
            return RedirectToAction(nameof(Invoices));
        }

        // POST: /Admin/MarkPaid  (e.g. patient paid at the counter)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id, string paymentMethod)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.InvoiceStatus != "Pending")
            {
                TempData["Error"] = "Only pending invoices can be marked as paid.";
                return RedirectToAction(nameof(EditInvoice), new { id });
            }

            var method = paymentMethod is "Cash" or "Card" ? paymentMethod : "Cash";
            var now = DateTime.Now;

            invoice.InvoiceStatus = "Paid";
            invoice.InvoicePaymentDate = now;

            _context.Payments.Add(new Payment
            {
                AmountPaid = invoice.AmountToPay,
                PaymentDate = now,
                PaymentMethod = method,
                FkInvoiceId = invoice.InvoiceId,
                Fk_UserId = invoice.FkUserId
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Invoice #INV-{id} marked as paid ({method}).";
            return RedirectToAction(nameof(Invoices));
        }

        private async Task LoadCreateLists()
        {
            ViewBag.Patients = await _context.Patients
                .AsNoTracking()
                .OrderBy(p => p.PatientLastName).ThenBy(p => p.PatientFirstName)
                .ToListAsync();

            // Only prescriptions that haven't been invoiced yet.
            ViewBag.Prescriptions = await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.FkMedication)
                .Where(p => !p.Invoices.Any())
                .OrderByDescending(p => p.PrescriptionDate)
                .Take(500)
                .ToListAsync();
        }
    }
}
