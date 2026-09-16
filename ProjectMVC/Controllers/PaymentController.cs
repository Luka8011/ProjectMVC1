using ClassLibrary1.Data;
using ClassLibrary1.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HospitalDbContext _context;

        public PaymentController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Payments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userName = HttpContext.Session.GetString("UserName") ?? "User";

            var invoices = await _context.Invoices
                .Include(i => i.FkPatient)
                .Where(i => i.FkUserId == userId.Value || (i.FkPatient != null && i.FkPatient.FkUserId == userId.Value))
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();

            var viewModel = new PaymentModel
            {
                UserId = userId.Value,
                UserName = userName,
                TotalPendingBalance = invoices
                    .Where(i => string.Equals(i.InvoiceStatus, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(i.InvoiceStatus))
                    .Sum(i => i.AmountToPay ?? 0m),

                PendingInvoices = invoices
                    .Where(i => string.Equals(i.InvoiceStatus, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(i.InvoiceStatus))
                    .Select(i => new InvoiceItemViewModel
                    {
                        InvoiceId = i.InvoiceId,
                        Description = i.FkPrescriptionId != null ? $"Prescription Charge #{i.FkPrescriptionId}" : "Medical Fee",
                        AmountToPay = i.AmountToPay ?? 0m,
                        DueDate = i.DueDate,
                        InvoiceStatus = i.InvoiceStatus ?? "Pending",
                        PatientName = i.FkPatient != null ? $"{i.FkPatient.PatientFirstName} {i.FkPatient.PatientLastName}" : userName
                    }).ToList(),

                PaidInvoices = invoices
                    .Where(i => string.Equals(i.InvoiceStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                    .Select(i => new InvoiceItemViewModel
                    {
                        InvoiceId = i.InvoiceId,
                        Description = i.FkPrescriptionId != null ? $"Prescription Charge #{i.FkPrescriptionId}" : "Medical Fee",
                        AmountToPay = i.AmountToPay ?? 0m,
                        DueDate = i.DueDate,
                        InvoicePaymentDate = i.InvoicePaymentDate,
                        InvoiceStatus = i.InvoiceStatus ?? "Paid",  
                        PatientName = i.FkPatient != null ? $"{i.FkPatient.PatientFirstName} {i.FkPatient.PatientLastName}" : userName
                    }).ToList()
            };

            return View(viewModel);
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Invoices()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string userName = HttpContext.Session.GetString("UserName") ?? "User";

            var invoices = await _context.Invoices
                .Include(i => i.FkPatient)
                .Where(i => i.FkUserId == userId.Value || (i.FkPatient != null && i.FkPatient.FkUserId == userId.Value))
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();

            var viewModel = new PaymentModel
            {
                UserId = userId.Value,
                UserName = userName,
                TotalPendingBalance = invoices
                    .Where(i => string.Equals(i.InvoiceStatus, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(i.InvoiceStatus))
                    .Sum(i => i.AmountToPay ?? 0m),

                PendingInvoices = invoices
                    .Where(i => string.Equals(i.InvoiceStatus, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(i.InvoiceStatus))
                    .Select(i => new InvoiceItemViewModel
                    {
                        InvoiceId = i.InvoiceId,
                        Description = i.FkPrescriptionId != null ? $"Prescription Charge #{i.FkPrescriptionId}" : "Medical Fee",
                        AmountToPay = i.AmountToPay ?? 0m,
                        DueDate = i.DueDate,
                        InvoiceStatus = i.InvoiceStatus ?? "Pending",
                        PatientName = i.FkPatient != null ? $"{i.FkPatient.PatientFirstName} {i.FkPatient.PatientLastName}" : userName
                    }).ToList(),

                PaidInvoices = invoices
                    .Where(i => string.Equals(i.InvoiceStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                    .Select(i => new InvoiceItemViewModel
                    {
                        InvoiceId = i.InvoiceId,
                        Description = i.FkPrescriptionId != null ? $"Prescription Charge #{i.FkPrescriptionId}" : "Medical Fee",
                        AmountToPay = i.AmountToPay ?? 0m,
                        DueDate = i.DueDate,
                        InvoicePaymentDate = i.InvoicePaymentDate,
                        InvoiceStatus = i.InvoiceStatus ?? "Paid",
                        PatientName = i.FkPatient != null ? $"{i.FkPatient.PatientFirstName} {i.FkPatient.PatientLastName}" : userName
                    }).ToList()
            };

            // Pass the model to the view
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayInvoice(int invoiceId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.InvoiceStatus = "Paid";
            invoice.InvoicePaymentDate = DateTime.UtcNow;

            var payment = new Payment
            {
                FkInvoiceId = invoice.InvoiceId,
                Fk_UserId = userId.Value,
                AmountPaid = invoice.AmountToPay ?? 0m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Credit Card / Online",
            };

            _context.Invoices.Update(invoice);
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Payment of ${(invoice.AmountToPay ?? 0m):F2} for Invoice #{invoice.InvoiceId} processed successfully!";
            return RedirectToAction("Payments");
        }
    }
}