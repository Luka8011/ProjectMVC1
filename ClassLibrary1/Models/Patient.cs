using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string? PatientFirstName { get; set; }

    public string? PatientLastName { get; set; }

    public DateOnly? PatientDob { get; set; }

    public string? PatientGender { get; set; }

    public string? PatientBloodType { get; set; }

    public string? PatientMail { get; set; }

    public string? PatientPhoneNumber { get; set; }

    public string? PatientInsuranceEmail { get; set; }

    public int? FkUserId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User? FkUser { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
