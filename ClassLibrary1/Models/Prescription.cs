using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public DateTime? PrescriptionDate { get; set; }

    public string? Dosage { get; set; }

    public string? Frequency { get; set; }

    public int? DurationDays { get; set; }

    public int? FkPatientId { get; set; }

    public int? FkDoctorId { get; set; }

    public int? FkMedicationId { get; set; }

    public virtual Doctor? FkDoctor { get; set; }

    public virtual Medication? FkMedication { get; set; }

    public virtual Patient? FkPatient { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
