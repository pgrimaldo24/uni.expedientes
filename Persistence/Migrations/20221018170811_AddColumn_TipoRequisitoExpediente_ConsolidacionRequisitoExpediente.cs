using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddColumn_TipoRequisitoExpediente_ConsolidacionRequisitoExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_TipoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "TipoRequisitoExpedienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidacionesRequisitosExpedientes_TiposRequisitosExpedientes_TipoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "TipoRequisitoExpedienteId",
                principalTable: "TiposRequisitosExpedientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidacionesRequisitosExpedientes_TiposRequisitosExpedientes_TipoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_TipoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes");

            migrationBuilder.DropColumn(
                name: "TipoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes");
        }
    }
}
