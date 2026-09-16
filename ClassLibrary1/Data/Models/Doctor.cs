using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ClassLibrary1.Data.Models;

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
    [StringLength(100)]
    public string? WorkHours { get; set; }
    [AllowNull]
    public string? AvatarPath { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Department? FkDepartment { get; set; }

    public virtual User? FkUser { get; set; }

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
