using System;
using System.Collections.Generic;

namespace ClassLibrary1.Data.Models;

public partial class LabOrder
{
    public int LabOrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? OrderStatus { get; set; }

    public int FkPatientId { get; set; }

    public int FkDoctorId { get; set; }

    public int? FkAppointmentId { get; set; }

    public virtual Appointment? FkAppointment { get; set; }

    public virtual Doctor FkDoctor { get; set; } = null!;

    public virtual Patient FkPatient { get; set; } = null!;

    public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
}
