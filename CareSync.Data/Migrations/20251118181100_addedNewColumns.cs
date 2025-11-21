using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSync.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class addedNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "T_DoctorDetails");

            migrationBuilder.AddColumn<string>(
                name: "ArabicFirstName",
                table: "T_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArabicLastName",
                table: "T_Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArabicLabAddress",
                table: "T_Labs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArabicLabName",
                table: "T_Labs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabAddress",
                table: "T_Labs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartTime",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndTime",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AvailableDays",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArabicClinicAddress",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArabicSpecialization",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicAddress",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArabicFirstName",
                table: "T_Users");

            migrationBuilder.DropColumn(
                name: "ArabicLastName",
                table: "T_Users");

            migrationBuilder.DropColumn(
                name: "ArabicLabAddress",
                table: "T_Labs");

            migrationBuilder.DropColumn(
                name: "ArabicLabName",
                table: "T_Labs");

            migrationBuilder.DropColumn(
                name: "LabAddress",
                table: "T_Labs");

            migrationBuilder.DropColumn(
                name: "ArabicClinicAddress",
                table: "T_DoctorDetails");

            migrationBuilder.DropColumn(
                name: "ArabicSpecialization",
                table: "T_DoctorDetails");

            migrationBuilder.DropColumn(
                name: "ClinicAddress",
                table: "T_DoctorDetails");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "T_DoctorDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "T_DoctorDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AvailableDays",
                table: "T_DoctorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "T_DoctorDetails",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
