using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceWorkHoursWithInterval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndsWorkAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "StartsWorkAt",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "WorkHours",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkHours",
                table: "Doctors");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndsWorkAt",
                table: "Doctors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartsWorkAt",
                table: "Doctors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
