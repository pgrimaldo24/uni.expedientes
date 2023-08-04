using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_Anotaciones_RolesAnotaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anotaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRefCuentaSeguridad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EsPublica = table.Column<bool>(type: "bit", nullable: false),
                    EsRestringida = table.Column<bool>(type: "bit", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRefCuentaSeguridadModificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anotaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anotaciones_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolesAnotaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnotacionId = table.Column<int>(type: "int", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesAnotaciones", x => x.Id);
                    table.UniqueConstraint("AK_RolesAnotaciones_AnotacionId_Rol", x => new { x.AnotacionId, x.Rol });
                    table.ForeignKey(
                        name: "FK_RolesAnotaciones_Anotaciones_AnotacionId",
                        column: x => x.AnotacionId,
                        principalTable: "Anotaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_ExpedienteAlumnoId",
                table: "Anotaciones",
                column: "ExpedienteAlumnoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolesAnotaciones");

            migrationBuilder.DropTable(
                name: "Anotaciones");
        }
    }
}
