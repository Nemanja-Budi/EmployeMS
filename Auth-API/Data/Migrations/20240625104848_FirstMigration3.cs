using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADMitroSremEmploye.Data.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnualLeaves_AspNetUsers_CreatedByUserId",
                table: "AnnualLeaves");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "AnnualLeaves",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnualLeaves_AspNetUsers_CreatedByUserId",
                table: "AnnualLeaves",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnualLeaves_AspNetUsers_CreatedByUserId",
                table: "AnnualLeaves");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "AnnualLeaves",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnualLeaves_AspNetUsers_CreatedByUserId",
                table: "AnnualLeaves",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
