using ClassLibrary1.Data;
using ClassLibrary1.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalDbContext _context;

        public DoctorsController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> DoctorsView(string? searchString, string? selectedSpecialization)
        {
            ViewBag.Specializations = await _context.Doctors
                .Where(d => !string.IsNullOrEmpty(d.DoctorSpecialization))
                .Select(d => d.DoctorSpecialization)
                .Distinct()
                .ToListAsync();

            ViewData["CurrentFilter"] = searchString;

            var doctorsQuery = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                doctorsQuery = doctorsQuery.Where(d =>
                    d.DoctorFirstName.Contains(searchString) ||
                    d.DoctorLastName.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(selectedSpecialization))
            {
                doctorsQuery = doctorsQuery.Where(d => d.DoctorSpecialization == selectedSpecialization);
            }

            var doctors = await doctorsQuery.ToListAsync();

            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> Appointments(int? doctorId, DateTime? date)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userRole == "Doctor")
            {
                string? userEmail = HttpContext.Session.GetString("UserName");

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var loggedInDoctor = await _context.Doctors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.FkUserId == userId || d.DoctorMail == userEmail);

                if (loggedInDoctor == null)
                {
                    return NotFound("Doctor record not found for the logged-in user.");
                }

                var doctorAppointments = await _context.Appointments
                    .Include(a => a.FkPatient)
                    .Where(a => a.FkDoctorId == loggedInDoctor.DoctorId)
                    .OrderBy(a => a.AppointmentDate)
                    .AsNoTracking()
                    .ToListAsync();

                ViewBag.DoctorAppointments = doctorAppointments;
                return View(loggedInDoctor);
            }

            if (userId.HasValue)
            {
                ViewBag.UserAppointments = await _context.Appointments
                    .Include(a => a.FkDoctor)
                    .Include(a => a.FkPatient)
                    .Where(a => a.FkPatient != null && a.FkPatient.FkUserId == userId.Value)
                    .OrderByDescending(a => a.AppointmentDate)
                    .AsNoTracking()
                    .ToListAsync();
            }

            if (doctorId.HasValue)
            {
                var doctor = await _context.Doctors.FindAsync(doctorId.Value);
                if (doctor == null) return NotFound();

                DateTime selectedDate = date ?? DateTime.Today;
                var doctorModel = new DoctorModel(_context);

                ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
                ViewBag.AvailableSlots = await doctorModel.GetAvailableSlotsAsync(doctorId.Value, selectedDate);

                return View(doctor);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(
            Appointment appointment,
            string AppointmentDateOnly,
            string AppointmentTimeSlot,
            string PatientFirstName,
            string PatientLastName,
            string PatientEmail,
            string PatientPhone,
            string PatientBloodType,
            int FkDoctorId,
            DateTime PatientDOB)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!string.IsNullOrEmpty(AppointmentDateOnly) && !string.IsNullOrEmpty(AppointmentTimeSlot))
            {
                var startTimeStr = AppointmentTimeSlot.Split('-')[0].Trim();
                if (DateTime.TryParse($"{AppointmentDateOnly} {startTimeStr}", out DateTime parsedDateTime))
                {
                    appointment.AppointmentDate = parsedDateTime;
                }
            }

            var patient = new Patient
            {
                PatientFirstName = PatientFirstName,
                PatientLastName = PatientLastName,
                PatientMail = PatientEmail,
                PatientPhoneNumber = PatientPhone,
                PatientDob = DateOnly.FromDateTime(PatientDOB),
                FkUserId = userId
            };

            if (!string.IsNullOrWhiteSpace(PatientBloodType))
            {
                patient.PatientBloodType = PatientBloodType;
            }

            var doctor = await _context.Doctors.FindAsync(FkDoctorId);

            appointment.FkDoctorId = FkDoctorId;
            appointment.FkPatient = patient;
            appointment.FkDoctor = doctor;

            ModelState.Remove("FkDoctor");
            ModelState.Remove("FkPatient");
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");

            if (ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                _context.Appointments.Add(appointment);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your appointment has been successfully booked!";
                return RedirectToAction("Appointments");
            }

            if (doctor == null) return NotFound();

            DateTime selectedDate = appointment.AppointmentDate.HasValue
                ? appointment.AppointmentDate.Value.Date
                : DateTime.Today;

            var doctorModel = new DoctorModel(_context);
            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.AvailableSlots = await doctorModel.GetAvailableSlotsAsync(appointment.FkDoctorId ?? doctor.DoctorId, selectedDate);

            if (userId.HasValue)
            {
                ViewBag.UserAppointments = await _context.Appointments
                    .Include(a => a.FkDoctor)
                    .Include(a => a.FkPatient)
                    .Where(a => a.FkPatient != null && a.FkPatient.FkUserId == userId.Value)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();
            }

            return View("Appointments", doctor);
        }
    }
}