using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class DeleteColumn_IdRefAlumno_ExpedienteAlumno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdRefAlumno",
                table: "ExpedienteAlumno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdRefAlumno",
                table: "ExpedienteAlumno",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
