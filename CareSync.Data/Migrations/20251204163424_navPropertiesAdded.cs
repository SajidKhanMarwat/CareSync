using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSync.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class navPropertiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentID",
                table: "T_LabReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "T_LabRequestsLabRequestID",
                table: "T_LabReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_LabReports_T_LabRequestsLabRequestID",
                table: "T_LabReports",
                column: "T_LabRequestsLabRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_T_LabReports_T_LabRequests_T_LabRequestsLabRequestID",
                table: "T_LabReports",
                column: "T_LabRequestsLabRequestID",
                principalTable: "T_LabRequests",
                principalColumn: "LabRequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_LabReports_T_LabRequests_T_LabRequestsLabRequestID",
                table: "T_LabReports");

            migrationBuilder.DropIndex(
                name: "IX_T_LabReports_T_LabRequestsLabRequestID",
                table: "T_LabReports");

            migrationBuilder.DropColumn(
                name: "AppointmentID",
                table: "T_LabReports");

            migrationBuilder.DropColumn(
                name: "T_LabRequestsLabRequestID",
                table: "T_LabReports");
        }
    }
}
