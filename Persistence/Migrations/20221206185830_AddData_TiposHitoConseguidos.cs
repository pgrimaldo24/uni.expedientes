using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddData_TiposHitoConseguidos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 15)
	                INSERT INTO [dbo].[TiposHitoConseguidos] (Id, Nombre, Icono) VALUES (15, 'Anulado', 'far fa-times-circle');
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 13)
	                INSERT INTO [dbo].[TiposHitoConseguidos] (Id, Nombre, Icono) VALUES (13, 'Desestimado', 'fas fa-heart-broken');
                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 8)
	                INSERT INTO [dbo].[TipoSeguimientoExpediente] (IdTipoSeguimientoExpediente, Nombre) VALUES (8,'Expediente cerrado');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
