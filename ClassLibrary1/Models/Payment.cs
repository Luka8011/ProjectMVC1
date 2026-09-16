using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal? AmountPaid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public int? FkInvoiceId { get; set; }

    public virtual Invoice? FkInvoice { get; set; }
}
