using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_Tareas_And_TareasDetalle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tareas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Completadas = table.Column<int>(type: "int", nullable: false),
                    Fallidas = table.Column<int>(type: "int", nullable: false),
                    IdRefUniversidad = table.Column<int>(type: "int", nullable: true),
                    IdRefEstudio = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TareasDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TareaId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletadaOk = table.Column<bool>(type: "bit", nullable: false),
                    Mensaje = table.Column<string>(type: "varchar(1500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareasDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TareasDetalle_ExpedienteAlumno_ExpedienteId",
                        column: x => x.ExpedienteId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareasDetalle_Tareas",
                        column: x => x.TareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TareasDetalle_ExpedienteId",
                table: "TareasDetalle",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TareasDetalle_TareaId",
                table: "TareasDetalle",
                column: "TareaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TareasDetalle");

            migrationBuilder.DropTable(
                name: "Tareas");
        }
    }
}
