using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserPassword { get; set; }

    public string? UserEmail { get; set; }

    public string? UserRole { get; set; }

    public string? UserName { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
