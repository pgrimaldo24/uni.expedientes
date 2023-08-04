using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddData_TipoSeguimientoExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 3)
	                INSERT INTO [dbo].[TipoSeguimientoExpediente] ([IdTipoSeguimientoExpediente], [Nombre]) VALUES (3, 'Expediente modificado vía de acceso')
                ELSE
                    UPDATE [dbo].[TipoSeguimientoExpediente] SET [Nombre] = 'Expediente modificado vía de acceso' WHERE [IdTipoSeguimientoExpediente] = 3

                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 4)
	                INSERT INTO [dbo].[TipoSeguimientoExpediente] ([IdTipoSeguimientoExpediente], [Nombre]) VALUES (4, 'Expediente modificado titulación de acceso')
                ELSE
                    UPDATE [dbo].[TipoSeguimientoExpediente] SET [Nombre] = 'Expediente modificado titulación de acceso' WHERE [IdTipoSeguimientoExpediente] = 4

                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 5)
	                INSERT INTO [dbo].[TipoSeguimientoExpediente] ([IdTipoSeguimientoExpediente], [Nombre]) VALUES (5, 'Expediente modificado tipo de vinculación')
                ELSE
                    UPDATE [dbo].[TipoSeguimientoExpediente] SET [Nombre] = 'Expediente modificado tipo de vinculación' WHERE [IdTipoSeguimientoExpediente] = 5
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
