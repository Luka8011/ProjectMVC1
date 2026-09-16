using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
