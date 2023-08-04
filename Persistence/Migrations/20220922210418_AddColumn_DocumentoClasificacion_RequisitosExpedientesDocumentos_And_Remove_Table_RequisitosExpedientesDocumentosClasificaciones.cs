using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddColumn_DocumentoClasificacion_RequisitosExpedientesDocumentos_And_Remove_Table_RequisitosExpedientesDocumentosClasificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequisitosExpedientesDocumentosClasificaciones");

            migrationBuilder.AddColumn<string>(
                name: "DocumentoClasificacion",
                table: "RequisitosExpedientesDocumentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentoClasificacion",
                table: "RequisitosExpedientesDocumentos");

            migrationBuilder.CreateTable(
                name: "RequisitosExpedientesDocumentosClasificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitoExpedienteDocumentoId = table.Column<int>(type: "int", nullable: true),
                    IdRefClasificacionDocumental = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdRefUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosExpedientesDocumentosClasificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientesDocumentosClasificaciones_RequisitosExpedientesDocumentos_RequisitoExpedienteDocumentoId",
                        column: x => x.RequisitoExpedienteDocumentoId,
                        principalTable: "RequisitosExpedientesDocumentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientesDocumentosClasificaciones_RequisitoExpedienteDocumentoId",
                table: "RequisitosExpedientesDocumentosClasificaciones",
                column: "RequisitoExpedienteDocumentoId");
        }
    }
}
