using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class UpdateTableRequisitoExpedienteFileType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdRefClasificacionDocumental",
                table: "RequisitosExpedientesFileType");

            migrationBuilder.RenameColumn(
                name: "IdRefUniversidad",
                table: "RequisitosExpedientesFileType",
                newName: "IdRefFileType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdRefFileType",
                table: "RequisitosExpedientesFileType",
                newName: "IdRefUniversidad");

            migrationBuilder.AddColumn<string>(
                name: "IdRefClasificacionDocumental",
                table: "RequisitosExpedientesFileType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
