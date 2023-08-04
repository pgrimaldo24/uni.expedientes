using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_RequisitosExpedientesRequerimientosTitulos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequisitosExpedientesRequerimientosTitulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequiereMatricularse = table.Column<bool>(type: "bit", nullable: false),
                    TipoRelacionExpedienteId = table.Column<int>(type: "int", nullable: false),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosExpedientesRequerimientosTitulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientesRequerimientosTitulos_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientesRequerimientosTitulos_TiposRelacionesExpediente_TipoRelacionExpedienteId",
                        column: x => x.TipoRelacionExpedienteId,
                        principalTable: "TiposRelacionesExpediente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientesRequerimientosTitulos_RequisitoExpedienteId",
                table: "RequisitosExpedientesRequerimientosTitulos",
                column: "RequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientesRequerimientosTitulos_TipoRelacionExpedienteId",
                table: "RequisitosExpedientesRequerimientosTitulos",
                column: "TipoRelacionExpedienteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequisitosExpedientesRequerimientosTitulos");
        }
    }
}
