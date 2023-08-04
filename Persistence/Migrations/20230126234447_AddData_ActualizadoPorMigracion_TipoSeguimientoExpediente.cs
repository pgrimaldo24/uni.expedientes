using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddData_ActualizadoPorMigracion_TipoSeguimientoExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 9)
                   INSERT INTO [dbo].[TipoSeguimientoExpediente] ([IdTipoSeguimientoExpediente], [Nombre]) VALUES (9, 'Actualizado por migración')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
