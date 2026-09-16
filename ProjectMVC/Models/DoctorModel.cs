using ClassLibrary1.Data;
using ClassLibrary1.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ProjectMVC.Models
{
    public class DoctorModel
    {
        private readonly HospitalDbContext _context;

        public DoctorModel(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime selectedDate)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);

            if (doctor == null || string.IsNullOrWhiteSpace(doctor.WorkHours))
            {
                return new List<DateTime>();
            }

            var parts = doctor.WorkHours.Split('-', '–');
            if (parts.Length != 2 ||
                !TimeSpan.TryParse(parts[0].Trim(), CultureInfo.InvariantCulture, out TimeSpan startTime) ||
                !TimeSpan.TryParse(parts[1].Trim(), CultureInfo.InvariantCulture, out TimeSpan endTime))
            {
                startTime = new TimeSpan(8, 0, 0);
                endTime = new TimeSpan(16, 0, 0);
            }

            DateTime shiftStart = selectedDate.Date.Add(startTime);
            DateTime shiftEnd = selectedDate.Date.Add(endTime);

            var bookedTimes = await _context.Appointments
                .Where(a => a.FkDoctorId == doctorId && a.AppointmentDate == selectedDate.Date)
                .Select(a => a.AppointmentDate)
                .ToListAsync();

            var availableSlots = new List<DateTime>();

            for (DateTime slot = shiftStart; slot < shiftEnd; slot = slot.AddHours(1))
            {
                bool isBooked = bookedTimes.Any(booked => Math.Abs((booked - slot).Value.TotalMinutes) < 60);

                bool isPast = selectedDate.Date == DateTime.Today && slot <= DateTime.Now;

                if (!isBooked && !isPast)
                {
                    availableSlots.Add(slot);
                }
            }

            return availableSlots;
        }
    }
}
