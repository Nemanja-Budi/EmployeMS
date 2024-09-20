using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADMitroSremEmploye.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeletePovratnicaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PovratnicaStavke");

            migrationBuilder.DropTable(
                name: "Povratnica");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Povratnica",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DokumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KomintentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Povratnica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Povratnica_Dokument_DokumentId",
                        column: x => x.DokumentId,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Povratnica_Komintenti_KomintentId",
                        column: x => x.KomintentId,
                        principalTable: "Komintenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PovratnicaStavke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProizvodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UlaznaKolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UlaznaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PovratnicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PovratnicaStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PovratnicaStavke_Povratnica_PovratnicaId",
                        column: x => x.PovratnicaId,
                        principalTable: "Povratnica",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PovratnicaStavke_Proizvod_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Povratnica_DokumentId",
                table: "Povratnica",
                column: "DokumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Povratnica_KomintentId",
                table: "Povratnica",
                column: "KomintentId");

            migrationBuilder.CreateIndex(
                name: "IX_PovratnicaStavke_PovratnicaId",
                table: "PovratnicaStavke",
                column: "PovratnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_PovratnicaStavke_ProizvodId",
                table: "PovratnicaStavke",
                column: "ProizvodId");
        }
    }
}
