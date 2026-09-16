using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Medication
{
    public int MedicationId { get; set; }

    public string? MedicationName { get; set; }

    public string? MedicationDescription { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? StockQuantity { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
