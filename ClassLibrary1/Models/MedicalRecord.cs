using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class MedicalRecord
{
    public int RecordId { get; set; }

    public string? RecordDescription { get; set; }

    public DateTime? RecordCreationTime { get; set; }

    public int? FkPatientId { get; set; }

    public virtual Patient? FkPatient { get; set; }
}
