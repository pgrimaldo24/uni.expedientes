using Microsoft.EntityFrameworkCore.Migrations;

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class UpdateTable_ExpedientesEspecializaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpedientesEspecializaciones",
                table: "ExpedientesEspecializaciones");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ExpedientesEspecializaciones_ExpedienteAlumnoId_IdRefEspecializacion",
                table: "ExpedientesEspecializaciones",
                columns: new[] { "ExpedienteAlumnoId", "IdRefEspecializacion" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpedientesEspecializaciones",
                table: "ExpedientesEspecializaciones",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ExpedientesEspecializaciones_ExpedienteAlumnoId_IdRefEspecializacion",
                table: "ExpedientesEspecializaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpedientesEspecializaciones",
                table: "ExpedientesEspecializaciones");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpedientesEspecializaciones",
                table: "ExpedientesEspecializaciones",
                columns: new[] { "ExpedienteAlumnoId", "IdRefEspecializacion" });
        }
    }
}
