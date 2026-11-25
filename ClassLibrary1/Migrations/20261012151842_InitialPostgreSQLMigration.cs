using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ClassLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQLMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__B2079BCDE99A1AE4", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    LabTestID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TestDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ReferenceRange = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TestCost = table.Column<decimal>(type: "numeric(9,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LabTests__64D339C56514A0B5", x => x.LabTestID);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    MedicationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedicationName = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    MedicationDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(9,2)", nullable: true),
                    StockQuantity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Medicati__62EC1ADA74AE1DAE", x => x.MedicationID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserPassword = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UserEmail = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    UserRole = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    UserName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCACC982AC5D", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoctorFirstName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DoctorLastName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DoctorSpecialization = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    DoctorLicenseNumber = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: true),
                    DoctorMail = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    FK_DepartmentID = table.Column<int>(type: "integer", nullable: true),
                    FK_UserID = table.Column<int>(type: "integer", nullable: true),
                    WorkHours = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AvatarPath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Doctors__2DC00EDF89646B08", x => x.DoctorID);
                    table.ForeignKey(
                        name: "FK__Doctors__FK_Depa__4E88ABD4",
                        column: x => x.FK_DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK__Doctors__FK_User__4F7CD00D",
                        column: x => x.FK_UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientFirstName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PatientLastName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    PatientDOB = table.Column<DateOnly>(type: "date", nullable: true),
                    PatientGender = table.Column<string>(type: "character varying(6)", unicode: false, maxLength: 6, nullable: true),
                    PatientBloodType = table.Column<string>(type: "character varying(5)", unicode: false, maxLength: 5, nullable: true),
                    PatientMail = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    PatientPhoneNumber = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    PatientInsuranceEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FK_UserID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patients__970EC346961EE700", x => x.PatientID);
                    table.ForeignKey(
                        name: "FK__Patients__FK_Use__52593CB8",
                        column: x => x.FK_UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AppointmentRoomNumber = table.Column<int>(type: "integer", nullable: true),
                    FK_AppointmentDepartmentID = table.Column<int>(type: "integer", nullable: true),
                    FK_PatientID = table.Column<int>(type: "integer", nullable: true),
                    FK_DoctorID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__8ECDFCA21BEE4A3C", x => x.AppointmentID);
                    table.ForeignKey(
                        name: "FK__Appointme__FK_Ap__656C112C",
                        column: x => x.FK_AppointmentDepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK__Appointme__FK_Do__6754599E",
                        column: x => x.FK_DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "DoctorID");
                    table.ForeignKey(
                        name: "FK__Appointme__FK_Pa__66603565",
                        column: x => x.FK_PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID");
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordDescription = table.Column<string>(type: "character varying(400)", unicode: false, maxLength: 400, nullable: true),
                    RecordCreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FK_PatientID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MedicalR__FBDF78C942EF45A1", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK__MedicalRe__FK_Pa__5535A963",
                        column: x => x.FK_PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID");
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrescriptionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Dosage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DurationDays = table.Column<int>(type: "integer", nullable: true),
                    FK_PatientID = table.Column<int>(type: "integer", nullable: true),
                    FK_DoctorID = table.Column<int>(type: "integer", nullable: true),
                    FK_MedicationID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prescrip__40130812655E7AFF", x => x.PrescriptionID);
                    table.ForeignKey(
                        name: "FK__Prescript__FK_Do__5AEE82B9",
                        column: x => x.FK_DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "DoctorID");
                    table.ForeignKey(
                        name: "FK__Prescript__FK_Me__5BE2A6F2",
                        column: x => x.FK_MedicationID,
                        principalTable: "Medications",
                        principalColumn: "MedicationID");
                    table.ForeignKey(
                        name: "FK__Prescript__FK_Pa__59FA5E80",
                        column: x => x.FK_PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID");
                });

            migrationBuilder.CreateTable(
                name: "LabOrders",
                columns: table => new
                {
                    LabOrderID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    OrderStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValue: "Requested"),
                    FK_PatientID = table.Column<int>(type: "integer", nullable: false),
                    FK_DoctorID = table.Column<int>(type: "integer", nullable: false),
                    FK_AppointmentID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LabOrder__9CBC009E03F1B2CB", x => x.LabOrderID);
                    table.ForeignKey(
                        name: "FK__LabOrders__FK_Ap__70DDC3D8",
                        column: x => x.FK_AppointmentID,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentID");
                    table.ForeignKey(
                        name: "FK__LabOrders__FK_Do__6FE99F9F",
                        column: x => x.FK_DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "DoctorID");
                    table.ForeignKey(
                        name: "FK__LabOrders__FK_Pa__6EF57B66",
                        column: x => x.FK_PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmountToPay = table.Column<decimal>(type: "numeric(9,2)", nullable: true),
                    InvoiceCreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InvoicePaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InvoiceStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FK_PrescriptionID = table.Column<int>(type: "integer", nullable: true),
                    FK_PatientID = table.Column<int>(type: "integer", nullable: true),
                    FK_UserID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoices__D796AAD54162723A", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK__Invoices__FK_Pat__5FB337D6",
                        column: x => x.FK_PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID");
                    table.ForeignKey(
                        name: "FK__Invoices__FK_Pre__5EBF139D",
                        column: x => x.FK_PrescriptionID,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionID");
                });

            migrationBuilder.CreateTable(
                name: "LabResults",
                columns: table => new
                {
                    LabResultID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ResultDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FK_LabOrderID = table.Column<int>(type: "integer", nullable: false),
                    FK_LabTestID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LabResul__3CEBE396E48B3DD9", x => x.LabResultID);
                    table.ForeignKey(
                        name: "FK__LabResult__FK_La__74AE54BC",
                        column: x => x.FK_LabOrderID,
                        principalTable: "LabOrders",
                        principalColumn: "LabOrderID");
                    table.ForeignKey(
                        name: "FK__LabResult__FK_La__75A278F5",
                        column: x => x.FK_LabTestID,
                        principalTable: "LabTests",
                        principalColumn: "LabTestID");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmountPaid = table.Column<decimal>(type: "numeric(9,2)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FK_InvoiceID = table.Column<int>(type: "integer", nullable: true),
                    FK_UserID = table.Column<int>(type: "integer", nullable: true),
                    FkUserUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__9B556A58D7D352F9", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payments_Users_FkUserUserId",
                        column: x => x.FkUserUserId,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Payments__FK_Inv__628FA481",
                        column: x => x.FK_InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FK_AppointmentDepartmentID",
                table: "Appointments",
                column: "FK_AppointmentDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FK_DoctorID",
                table: "Appointments",
                column: "FK_DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FK_PatientID",
                table: "Appointments",
                column: "FK_PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_FK_DepartmentID",
                table: "Doctors",
                column: "FK_DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_FK_UserID",
                table: "Doctors",
                column: "FK_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FK_PatientID",
                table: "Invoices",
                column: "FK_PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FK_PrescriptionID",
                table: "Invoices",
                column: "FK_PrescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_FK_AppointmentID",
                table: "LabOrders",
                column: "FK_AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_FK_DoctorID",
                table: "LabOrders",
                column: "FK_DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_FK_PatientID",
                table: "LabOrders",
                column: "FK_PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_FK_LabOrderID",
                table: "LabResults",
                column: "FK_LabOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_FK_LabTestID",
                table: "LabResults",
                column: "FK_LabTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_FK_PatientID",
                table: "MedicalRecords",
                column: "FK_PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FK_UserID",
                table: "Patients",
                column: "FK_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FK_InvoiceID",
                table: "Payments",
                column: "FK_InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FkUserUserId",
                table: "Payments",
                column: "FkUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_FK_DoctorID",
                table: "Prescriptions",
                column: "FK_DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_FK_MedicationID",
                table: "Prescriptions",
                column: "FK_MedicationID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_FK_PatientID",
                table: "Prescriptions",
                column: "FK_PatientID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabResults");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "LabOrders");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
