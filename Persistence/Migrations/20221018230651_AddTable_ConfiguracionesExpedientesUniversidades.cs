using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_ConfiguracionesExpedientesUniversidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesExpedientesUniversidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcronimoUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreUniversidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdIntegracionUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoDocumental = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TamanyoMaximoFichero = table.Column<int>(type: "int", nullable: false),
                    TiempoMaximoInactividad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesExpedientesUniversidades", x => x.Id);
                });

            migrationBuilder.Sql(@"
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('1', 'UNIR', 'Universidad Internacional de La Rioja', 'UNIR_ES', '1', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('2', 'UNIR Colombia', 'Fundación Universitaria Internacional de la Rioja - UNIR (Colombia)', 'FUNIR', '2', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('3', 'UNIR México', 'Universidad Internacional de la Rioja en México', 'UNIR_MX', '4', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('4', 'CUNIMAD', 'Centro Universitario Internacional de Madrid CUNIMAD', 'CUNIMAD', '3', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('5', 'EDIX', 'EDIX EDUCACIÓN, S.A', 'EDIX', '5', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('6', 'MIU', 'MIU CITY UNIVERSITY MIAMI', 'MIU', '6', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('7', 'EP NEWMAN', 'Escuela de Posgrado Newman', 'NEUMANN', '8', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('9', 'CPR FPE Edix', 'Centro Privado de Formación Profesional Específica Edix', 'ITT', '7', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('10', 'UO', 'Universidad de Otavalo', 'OTAVALO', '', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('11', 'UNI', 'Universidad InterNaciones, Guatemala', 'UNI', '', 10, 730);
                INSERT INTO [Expedientes].[dbo].[ConfiguracionesExpedientesUniversidades] (IdRefUniversidad, AcronimoUniversidad, NombreUniversidad, IdIntegracionUniversidad, CodigoDocumental, TamanyoMaximoFichero, TiempoMaximoInactividad) VALUES ('12', 'MIU MADRID', 'MIU City Campus Miami - Madrid', 'MIU MADRID', '', 10, 730);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesExpedientesUniversidades");
        }
    }
}
