using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddColumn_Icono_TiposHitoConseguidos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icono",
                table: "TiposHitoConseguidos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 14)
	                INSERT INTO [dbo].[TiposHitoConseguidos] (Id, Nombre, Icono) VALUES (14, 'Titulación', 'fas fa-award');

                Update TiposHitoConseguidos Set Icono = 'fas fa-edit' Where Id = 1;
                Update TiposHitoConseguidos Set Icono = 'fas fa-university' Where Id = 2;
                Update TiposHitoConseguidos Set Icono = 'fas fa-heart' Where Id = 3;
                Update TiposHitoConseguidos Set Icono = 'fas fa-user-times' Where Id = 4;
                Update TiposHitoConseguidos Set Icono = 'fas fa-user-lock' Where Id = 5;
                Update TiposHitoConseguidos Set Icono = 'fas fa-door-closed' Where Id = 6;
                Update TiposHitoConseguidos Set Icono = 'fab fa-openid' Where Id = 7;
                Update TiposHitoConseguidos Set Icono = 'fas fa-user-graduate' Where Id = 8;
                Update TiposHitoConseguidos Set Icono = 'fas fa-award' Where Id = 9;
                Update TiposHitoConseguidos Set Icono = 'fas fa-dollar-sign' Where Id = 10;
                Update TiposHitoConseguidos Set Icono = 'fas fa-gavel' Where Id = 11;
                Update TiposHitoConseguidos Set Icono = 'fas fa-medal' Where Id = 12;
                Update TiposHitoConseguidos Set Icono = 'fas fa-heart-broken', Nombre = 'Desestimado' Where Id = 13;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icono",
                table: "TiposHitoConseguidos");
        }
    }
}
