using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddColumn_CausaEstadoRequisitoConsolidadaExpedienteId_ConsolidacionRequisitoExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CausaEstadoRequisitoConsolidadaExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_CausaEstadoRequisitoConsolidadaExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "CausaEstadoRequisitoConsolidadaExpedienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsolidacionesRequisitosExpedientes_CausasEstadosRequisitosConsolidadasExpedientes_CausaEstadoRequisitoConsolidadaExpedient~",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "CausaEstadoRequisitoConsolidadaExpedienteId",
                principalTable: "CausasEstadosRequisitosConsolidadasExpedientes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsolidacionesRequisitosExpedientes_CausasEstadosRequisitosConsolidadasExpedientes_CausaEstadoRequisitoConsolidadaExpedient~",
                table: "ConsolidacionesRequisitosExpedientes");

            migrationBuilder.DropIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_CausaEstadoRequisitoConsolidadaExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes");

            migrationBuilder.DropColumn(
                name: "CausaEstadoRequisitoConsolidadaExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes");
        }
    }
}
