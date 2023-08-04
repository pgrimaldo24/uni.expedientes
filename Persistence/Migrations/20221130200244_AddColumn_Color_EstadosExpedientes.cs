using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddColumn_Color_EstadosExpedientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "EstadosExpedientes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                Update EstadosExpedientes Set Color = '#0000FF' Where Id = 1;
                Update EstadosExpedientes Set Color = '#008000' Where Id = 2;
                Update EstadosExpedientes Set Color = '#800080' Where Id = 3;
                Update EstadosExpedientes Set Color = '' Where Id = 4;
                Update EstadosExpedientes Set Color = '#FF8000' Where Id = 5;
                Update EstadosExpedientes Set Color = '#FF0000' Where Id = 6;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
