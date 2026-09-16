using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public decimal? AmountToPay { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string? InvoiceStatus { get; set; }

    public int? FkPrescriptionId { get; set; }

    public int? FkPatientId { get; set; }

    public virtual Patient? FkPatient { get; set; }

    public virtual Prescription? FkPrescription { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
