using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSync.DataLayer.Migrations;

/// <inheritdoc />
public partial class modifiedrule : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsActive",
            table: "T_Roles",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "RoleType",
            table: "T_Roles",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsActive",
            table: "T_Roles");

        migrationBuilder.DropColumn(
            name: "RoleType",
            table: "T_Roles");
    }
}
