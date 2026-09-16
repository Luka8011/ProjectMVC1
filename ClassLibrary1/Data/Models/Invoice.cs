using System;
using System.Collections.Generic;

namespace ClassLibrary1.Data.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public decimal? AmountToPay { get; set; }

    public DateTime? InvoiceCreationDate { get; set; }

    public DateTime? InvoicePaymentDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string? InvoiceStatus { get; set; }

    public int? FkPrescriptionId { get; set; }

    public int? FkPatientId { get; set; }

    public int? FkUserId { get; set; }


    public virtual Patient? FkPatient { get; set; }

    public virtual Prescription? FkPrescription { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
