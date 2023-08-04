using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTables_TitulacionesAccesos_Expedientes_Especializaciones_ParametrosConfiguracionesExpedientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParametrosConfiguracionesExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosConfiguracionesExpedientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TitulacionesAccesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InstitucionDocente = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NroSemestreRealizados = table.Column<int>(type: "int", nullable: true),
                    TipoEstudio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IdRefTerritorioInstitucionDocente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaInicioTitulo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechafinTitulo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CodigoColegiadoProfesional = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitulacionesAccesos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosConfiguracionesExpedientesFilesTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefFileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParametroConfiguracionExpedienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosConfiguracionesExpedientesFilesTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParametrosConfiguracionesExpedientesFilesTypes_ParametrosConfiguracionesExpedientes_ParametroConfiguracionExpedienteId",
                        column: x => x.ParametroConfiguracionExpedienteId,
                        principalTable: "ParametrosConfiguracionesExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                ALTER TABLE dbo.ExpedienteAlumno
                ADD IdRefAlumno NVARCHAR(50) NULL,
	                AlumnoNombre NVARCHAR(200) NULL,
	                AlumnoApellido1 NVARCHAR(200) NULL,
	                AlumnoApellido2 NVARCHAR(200) NULL,
	                IdRefTipoDocumentoIdentificacionPais NVARCHAR(50) NULL,
	                AlumnoNroDocIdentificacion NVARCHAR(50) NULL,
	                AlumnoEmail NVARCHAR(50) NULL,
	                IdRefViaAccesoPlan NVARCHAR(50) NULL,
	                DocAcreditativoViaAcceso NVARCHAR(MAX) NULL,
	                IdRefIntegracionDocViaAcceso NVARCHAR(50) NULL,
	                FechaSubidaDocViaAcceso NVARCHAR(50) NULL,
	                IdRefTipoVinculacion NVARCHAR(50) NULL,
	                NombrePlan NVARCHAR(200) NULL,
	                IdRefUniversidad NVARCHAR(50) NULL,
	                AcronimoUniversidad NVARCHAR(50) NULL,
	                IdRefCentro NVARCHAR(50) NULL,
	                IdRefAreaAcademica NVARCHAR(50) NULL,
	                IdRefTipoEstudio NVARCHAR(50) NULL,
	                IdRefEstudio NVARCHAR(50) NULL,
	                NombreEstudio NVARCHAR(200) NULL,
	                IdRefTitulo NVARCHAR(50) NULL,
	                FechaApertura DATETIME2 NULL,
	                FechaFinalizacion DATETIME2 NULL,
	                FechaTrabajoFinEstudio DATETIME2 NULL,
	                TituloTrabajoFinEstudio NVARCHAR(50) NULL,
	                FechaExpedicion DATETIME2 NULL,
	                NotaMedia FLOAT NULL,
	                FechaPago DATETIME2 NULL,
	                TitulacionAccesoId INT NULL;

                ALTER TABLE dbo.ExpedienteAlumno
                ADD CONSTRAINT FK_ExpedienteAlumno_TitulacionesAccesos_TitulacionAccesoId
                FOREIGN KEY (TitulacionAccesoId) REFERENCES TitulacionesAccesos(Id);
                ");

            migrationBuilder.CreateTable(
                name: "ExpedientesEspecializaciones",
                columns: table => new
                {
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false),
                    IdRefEspecializacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpedientesEspecializaciones", x => new { x.ExpedienteAlumnoId, x.IdRefEspecializacion });
                    table.ForeignKey(
                        name: "FK_ExpedientesEspecializaciones_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                ALTER TABLE dbo.SeguimientoExpediente
                ADD OrigenExterno NVARCHAR(250) NULL;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpedientesEspecializaciones");

            migrationBuilder.DropTable(
                name: "ParametrosConfiguracionesExpedientesFilesTypes");

            migrationBuilder.Sql(@"
                ALTER TABLE dbo.SeguimientoExpediente
                DROP COLUMN OrigenExterno;
                ");

            migrationBuilder.DropTable(
                name: "ParametrosConfiguracionesExpedientes");
            
            migrationBuilder.Sql(@"
                ALTER TABLE dbo.ExpedienteAlumno
                DROP CONSTRAINT FK_ExpedienteAlumno_TitulacionesAccesos_TitulacionAccesoId;

                ALTER TABLE dbo.ExpedienteAlumno
                DROP COLUMN IdRefAlumno,
	                AlumnoNombre,
	                AlumnoApellido1,
	                AlumnoApellido2,
	                IdRefTipoDocumentoIdentificacionPais,
	                AlumnoNroDocIdentificacion,
	                AlumnoEmail,
	                IdRefViaAccesoPlan,
	                DocAcreditativoViaAcceso,
	                IdRefIntegracionDocViaAcceso,
	                FechaSubidaDocViaAcceso,
	                IdRefTipoVinculacion,
	                NombrePlan,
	                IdRefUniversidad,
	                AcronimoUniversidad,
	                IdRefCentro,
	                IdRefAreaAcademica,
	                IdRefTipoEstudio,
	                IdRefEstudio,
	                NombreEstudio,
	                IdRefTitulo,
	                FechaApertura,
	                FechaFinalizacion,
	                FechaTrabajoFinEstudio,
	                TituloTrabajoFinEstudio,
	                FechaExpedicion,
	                NotaMedia,
	                FechaPago,
	                TitulacionAccesoId;
                ");

            migrationBuilder.DropTable(
                name: "TitulacionesAccesos");
        }
    }
}
