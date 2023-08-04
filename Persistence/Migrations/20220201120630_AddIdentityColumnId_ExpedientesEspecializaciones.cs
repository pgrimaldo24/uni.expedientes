using Microsoft.EntityFrameworkCore.Migrations;

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddIdentityColumnId_ExpedientesEspecializaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExpedientesEspecializaciones");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ExpedientesEspecializaciones",
                type: "int",
                nullable: false).Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "TituloTrabajoFinEstudio",
                table: "ExpedienteAlumno",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExpedientesEspecializaciones");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ExpedientesEspecializaciones",
                type: "int",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "TituloTrabajoFinEstudio",
                table: "ExpedienteAlumno",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);
        }
    }
}
