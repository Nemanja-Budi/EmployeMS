using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADMitroSremEmploye.Migrations
{
    /// <inheritdoc />
    public partial class SMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "CalculationMonth",
                table: "EmployeSalary",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "SettlementDate",
                table: "EmployeSalary",
                type: "date",
                nullable: false,
                defaultValue: DateOnly.FromDateTime(DateTime.Now)); // Promenio sam defaultValue ovde ako zelis trenutni datum
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationMonth",
                table: "EmployeSalary");

            migrationBuilder.DropColumn(
                name: "SettlementDate",
                table: "EmployeSalary");
        }
    }
}
