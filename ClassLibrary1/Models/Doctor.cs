using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string? DoctorFirstName { get; set; }

    public string? DoctorLastName { get; set; }

    public string? DoctorSpecialization { get; set; }

    public string? DoctorLicenseNumber { get; set; }

    public string? DoctorMail { get; set; }

    public int? FkDepartmentId { get; set; }

    public int? FkUserId { get; set; }

    public string? WorkHours { get; set; }

    public string? AvatarPath { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Department? FkDepartment { get; set; }

    public virtual User? FkUser { get; set; }

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
