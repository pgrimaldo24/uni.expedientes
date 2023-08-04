using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddColumnMensaje_SeguimientoExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mensaje",
                table: "SeguimientoExpediente",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mensaje",
                table: "SeguimientoExpediente");
        }
    }
}
