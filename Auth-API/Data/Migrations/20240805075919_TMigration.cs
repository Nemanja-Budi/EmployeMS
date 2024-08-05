using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADMitroSremEmploye.Data.Migrations
{
    /// <inheritdoc />
    public partial class TMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "AnnualLeaves",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "RequestDate",
                table: "AnnualLeaves",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "AnnualLeaves",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ApprovalDate",
                table: "AnnualLeaves",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "AnnualLeaves",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "AnnualLeaves",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "AnnualLeaves",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovalDate",
                table: "AnnualLeaves",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
