using System;
using System.Collections.Generic;

namespace ClassLibrary1.Data.Models;

public partial class LabResult
{
    public int LabResultId { get; set; }

    public string ResultValue { get; set; } = null!;

    public DateTime? ResultDate { get; set; }

    public string? Notes { get; set; }

    public int FkLabOrderId { get; set; }

    public int FkLabTestId { get; set; }

    public virtual LabOrder FkLabOrder { get; set; } = null!;

    public virtual LabTest FkLabTest { get; set; } = null!;
}
