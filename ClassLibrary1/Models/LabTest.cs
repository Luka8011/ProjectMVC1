using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class LabTest
{
    public int LabTestId { get; set; }

    public string TestName { get; set; } = null!;

    public string? TestDescription { get; set; }

    public string? ReferenceRange { get; set; }

    public decimal TestCost { get; set; }

    public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
}
