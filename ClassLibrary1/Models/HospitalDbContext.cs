using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1.Models;

public partial class HospitalDbContext : DbContext
{
    public HospitalDbContext()
    {
    }

    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<LabOrder> LabOrders { get; set; }

    public virtual DbSet<LabResult> LabResults { get; set; }

    public virtual DbSet<LabTest> LabTests { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=HospitalDatabase;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA21BEE4A3C");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.FkAppointmentDepartmentId).HasColumnName("FK_AppointmentDepartmentID");
            entity.Property(e => e.FkDoctorId).HasColumnName("FK_DoctorID");
            entity.Property(e => e.FkPatientId).HasColumnName("FK_PatientID");

            entity.HasOne(d => d.FkAppointmentDepartment).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.FkAppointmentDepartmentId)
                .HasConstraintName("FK__Appointme__FK_Ap__656C112C");

            entity.HasOne(d => d.FkDoctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.FkDoctorId)
                .HasConstraintName("FK__Appointme__FK_Do__6754599E");

            entity.HasOne(d => d.FkPatient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.FkPatientId)
                .HasConstraintName("FK__Appointme__FK_Pa__66603565");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCDE99A1AE4");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName).HasMaxLength(50);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EDF89646B08");

            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.DoctorFirstName).HasMaxLength(40);
            entity.Property(e => e.DoctorLastName).HasMaxLength(40);
            entity.Property(e => e.DoctorLicenseNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.DoctorMail).HasMaxLength(70);
            entity.Property(e => e.DoctorSpecialization)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FkDepartmentId).HasColumnName("FK_DepartmentID");
            entity.Property(e => e.FkUserId).HasColumnName("FK_UserID");
            entity.Property(e => e.WorkHours).HasMaxLength(100);

            entity.HasOne(d => d.FkDepartment).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.FkDepartmentId)
                .HasConstraintName("FK__Doctors__FK_Depa__4E88ABD4");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__Doctors__FK_User__4F7CD00D");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD54162723A");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.AmountToPay).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.FkPatientId).HasColumnName("FK_PatientID");
            entity.Property(e => e.FkPrescriptionId).HasColumnName("FK_PrescriptionID");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceStatus).HasMaxLength(20);

            entity.HasOne(d => d.FkPatient).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.FkPatientId)
                .HasConstraintName("FK__Invoices__FK_Pat__5FB337D6");

            entity.HasOne(d => d.FkPrescription).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.FkPrescriptionId)
                .HasConstraintName("FK__Invoices__FK_Pre__5EBF139D");
        });

        modelBuilder.Entity<LabOrder>(entity =>
        {
            entity.HasKey(e => e.LabOrderId).HasName("PK__LabOrder__9CBC009E03F1B2CB");

            entity.Property(e => e.LabOrderId).HasColumnName("LabOrderID");
            entity.Property(e => e.FkAppointmentId).HasColumnName("FK_AppointmentID");
            entity.Property(e => e.FkDoctorId).HasColumnName("FK_DoctorID");
            entity.Property(e => e.FkPatientId).HasColumnName("FK_PatientID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Requested");

            entity.HasOne(d => d.FkAppointment).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.FkAppointmentId)
                .HasConstraintName("FK__LabOrders__FK_Ap__70DDC3D8");

            entity.HasOne(d => d.FkDoctor).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.FkDoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabOrders__FK_Do__6FE99F9F");

            entity.HasOne(d => d.FkPatient).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.FkPatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabOrders__FK_Pa__6EF57B66");
        });

        modelBuilder.Entity<LabResult>(entity =>
        {
            entity.HasKey(e => e.LabResultId).HasName("PK__LabResul__3CEBE396E48B3DD9");

            entity.Property(e => e.LabResultId).HasColumnName("LabResultID");
            entity.Property(e => e.FkLabOrderId).HasColumnName("FK_LabOrderID");
            entity.Property(e => e.FkLabTestId).HasColumnName("FK_LabTestID");
            entity.Property(e => e.Notes).HasMaxLength(300);
            entity.Property(e => e.ResultDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ResultValue).HasMaxLength(100);

            entity.HasOne(d => d.FkLabOrder).WithMany(p => p.LabResults)
                .HasForeignKey(d => d.FkLabOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabResult__FK_La__74AE54BC");

            entity.HasOne(d => d.FkLabTest).WithMany(p => p.LabResults)
                .HasForeignKey(d => d.FkLabTestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabResult__FK_La__75A278F5");
        });

        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.LabTestId).HasName("PK__LabTests__64D339C56514A0B5");

            entity.Property(e => e.LabTestId).HasColumnName("LabTestID");
            entity.Property(e => e.ReferenceRange).HasMaxLength(100);
            entity.Property(e => e.TestCost).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.TestDescription).HasMaxLength(250);
            entity.Property(e => e.TestName).HasMaxLength(100);
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C942EF45A1");

            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.FkPatientId).HasColumnName("FK_PatientID");
            entity.Property(e => e.RecordCreationTime).HasColumnType("datetime");
            entity.Property(e => e.RecordDescription)
                .HasMaxLength(400)
                .IsUnicode(false);

            entity.HasOne(d => d.FkPatient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.FkPatientId)
                .HasConstraintName("FK__MedicalRe__FK_Pa__5535A963");
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.MedicationId).HasName("PK__Medicati__62EC1ADA74AE1DAE");

            entity.Property(e => e.MedicationId).HasColumnName("MedicationID");
            entity.Property(e => e.MedicationDescription).HasMaxLength(250);
            entity.Property(e => e.MedicationName).HasMaxLength(75);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(9, 2)");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346961EE700");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.FkUserId).HasColumnName("FK_UserID");
            entity.Property(e => e.PatientBloodType)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.PatientDob).HasColumnName("PatientDOB");
            entity.Property(e => e.PatientFirstName).HasMaxLength(40);
            entity.Property(e => e.PatientGender)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.PatientInsuranceEmail).HasMaxLength(50);
            entity.Property(e => e.PatientLastName).HasMaxLength(40);
            entity.Property(e => e.PatientMail).HasMaxLength(70);
            entity.Property(e => e.PatientPhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.FkUser).WithMany(p => p.Patients)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK__Patients__FK_Use__52593CB8");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58D7D352F9");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.FkInvoiceId).HasColumnName("FK_InvoiceID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(d => d.FkInvoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.FkInvoiceId)
                .HasConstraintName("FK__Payments__FK_Inv__628FA481");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__Prescrip__40130812655E7AFF");

            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.Dosage).HasMaxLength(50);
            entity.Property(e => e.FkDoctorId).HasColumnName("FK_DoctorID");
            entity.Property(e => e.FkMedicationId).HasColumnName("FK_MedicationID");
            entity.Property(e => e.FkPatientId).HasColumnName("FK_PatientID");
            entity.Property(e => e.Frequency).HasMaxLength(50);
            entity.Property(e => e.PrescriptionDate).HasColumnType("datetime");

            entity.HasOne(d => d.FkDoctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.FkDoctorId)
                .HasConstraintName("FK__Prescript__FK_Do__5AEE82B9");

            entity.HasOne(d => d.FkMedication).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.FkMedicationId)
                .HasConstraintName("FK__Prescript__FK_Me__5BE2A6F2");

            entity.HasOne(d => d.FkPatient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.FkPatientId)
                .HasConstraintName("FK__Prescript__FK_Pa__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACC982AC5D");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserEmail).HasMaxLength(70);
            entity.Property(e => e.UserName).HasMaxLength(60);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserRole).HasMaxLength(60);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
