using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_AsignaturasExpedientesRelacionadas_And_RemoveColumn_AsignaturaExpedienteId_Table_AsignaturasExpedientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AsignaturasExpedientes_AsignaturasExpedientes_AsignaturaExpedienteId",
                table: "AsignaturasExpedientes");

            migrationBuilder.DropIndex(
                name: "IX_AsignaturasExpedientes_AsignaturaExpedienteId",
                table: "AsignaturasExpedientes");

            migrationBuilder.DropColumn(
                name: "AsignaturaExpedienteId",
                table: "AsignaturasExpedientes");

            migrationBuilder.CreateTable(
                name: "AsignaturasExpedientesRelacionadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsignaturaExpedienteOrigenId = table.Column<int>(type: "int", nullable: false),
                    AsignaturaExpedienteDestinoId = table.Column<int>(type: "int", nullable: false),
                    Reconocida = table.Column<bool>(type: "bit", nullable: false),
                    Adaptada = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturasExpedientesRelacionadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientesRelacionadas_AsignaturasExpedientes_AsignaturaExpedienteDestinoId",
                        column: x => x.AsignaturaExpedienteDestinoId,
                        principalTable: "AsignaturasExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientesRelacionadas_AsignaturasExpedientes_AsignaturaExpedienteOrigenId",
                        column: x => x.AsignaturaExpedienteOrigenId,
                        principalTable: "AsignaturasExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasExpedientesRelacionadas_AsignaturaExpedienteDestinoId",
                table: "AsignaturasExpedientesRelacionadas",
                column: "AsignaturaExpedienteDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasExpedientesRelacionadas_AsignaturaExpedienteOrigenId",
                table: "AsignaturasExpedientesRelacionadas",
                column: "AsignaturaExpedienteOrigenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignaturasExpedientesRelacionadas");

            migrationBuilder.AddColumn<int>(
                name: "AsignaturaExpedienteId",
                table: "AsignaturasExpedientes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasExpedientes_AsignaturaExpedienteId",
                table: "AsignaturasExpedientes",
                column: "AsignaturaExpedienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_AsignaturasExpedientes_AsignaturasExpedientes_AsignaturaExpedienteId",
                table: "AsignaturasExpedientes",
                column: "AsignaturaExpedienteId",
                principalTable: "AsignaturasExpedientes",
                principalColumn: "Id");
        }
    }
}
