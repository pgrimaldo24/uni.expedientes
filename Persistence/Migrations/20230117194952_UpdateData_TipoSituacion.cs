using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class UpdateData_TipoSituacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                 IF EXISTS(SELECT * FROM [dbo].[TiposSituacionEstado] WHERE Id = 10)
                    UPDATE [dbo].[TiposSituacionEstado] SET Nombre = 'Variación matrícula realizada' WHERE Id = 10
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
