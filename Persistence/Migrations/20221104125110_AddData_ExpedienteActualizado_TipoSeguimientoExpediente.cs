using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddData_ExpedienteActualizado_TipoSeguimientoExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 7)
	                INSERT INTO [dbo].[TipoSeguimientoExpediente] ([IdTipoSeguimientoExpediente], [Nombre]) VALUES (7, 'Expediente actualizado')
                ELSE
                    UPDATE [dbo].[TipoSeguimientoExpediente] SET [Nombre] = 'Expediente actualizado' WHERE [IdTipoSeguimientoExpediente] = 7
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
