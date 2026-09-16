using System;
using System.Collections.Generic;

namespace ClassLibrary1.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public int? AppointmentRoomNumber { get; set; }

    public int? FkAppointmentDepartmentId { get; set; }

    public int? FkPatientId { get; set; }

    public int? FkDoctorId { get; set; }

    public virtual Department? FkAppointmentDepartment { get; set; }

    public virtual Doctor? FkDoctor { get; set; }

    public virtual Patient? FkPatient { get; set; }

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
}
