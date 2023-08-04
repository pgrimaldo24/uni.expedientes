using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_AsignaturasCalificaciones_TiposConvocatorias_EstadosCalificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosCalificaciones",
                columns: table => new
                {
                    IdEstadoCalificacion = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCalificaciones", x => x.IdEstadoCalificacion);
                });

            migrationBuilder.CreateTable(
                name: "TiposConvocatorias",
                columns: table => new
                {
                    IdTipoConvocatoria = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposConvocatorias", x => x.IdTipoConvocatoria);
                });

            migrationBuilder.CreateTable(
                name: "AsignaturasCalificaciones",
                columns: table => new
                {
                    IdAsignaturaCalificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaPublicado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaConfirmado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdRefPeriodoLectivo = table.Column<int>(type: "int", nullable: false),
                    Ciclo = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    AnyoAcademico = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Calificacion = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NombreCalificacion = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Convocatoria = table.Column<int>(type: "int", nullable: false),
                    IdRefAsignaturaMatriculada = table.Column<int>(type: "int", nullable: true),
                    IdRefAsignaturaOfertada = table.Column<int>(type: "int", nullable: true),
                    Plataforma = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IdRefGrupoCurso = table.Column<int>(type: "int", nullable: true),
                    IdPublicadorConfirmador = table.Column<int>(type: "int", nullable: true),
                    EsMatriculaHonor = table.Column<bool>(type: "bit", nullable: false),
                    EsNoPresentado = table.Column<bool>(type: "bit", nullable: false),
                    Superada = table.Column<bool>(type: "bit", nullable: false),
                    TipoConvocatoriaId = table.Column<int>(type: "int", nullable: false),
                    EstadoCalificacionId = table.Column<int>(type: "int", nullable: false),
                    AsignaturaExpedienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturasCalificaciones", x => x.IdAsignaturaCalificacion);
                    table.ForeignKey(
                        name: "FK_AsignaturasCalificaciones_AsignaturasExpedientes_AsignaturaExpedienteId",
                        column: x => x.AsignaturaExpedienteId,
                        principalTable: "AsignaturasExpedientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AsignaturasCalificaciones_EstadosCalificaciones_EstadoCalificacionId",
                        column: x => x.EstadoCalificacionId,
                        principalTable: "EstadosCalificaciones",
                        principalColumn: "IdEstadoCalificacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignaturasCalificaciones_TiposConvocatorias_TipoConvocatoriaId",
                        column: x => x.TipoConvocatoriaId,
                        principalTable: "TiposConvocatorias",
                        principalColumn: "IdTipoConvocatoria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasCalificaciones_AsignaturaExpedienteId",
                table: "AsignaturasCalificaciones",
                column: "AsignaturaExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasCalificaciones_EstadoCalificacionId",
                table: "AsignaturasCalificaciones",
                column: "EstadoCalificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasCalificaciones_TipoConvocatoriaId",
                table: "AsignaturasCalificaciones",
                column: "TipoConvocatoriaId");

            migrationBuilder.Sql(@"
                INSERT INTO [Expedientes].[dbo].[EstadosCalificaciones] (IdEstadoCalificacion, Nombre) VALUES (1, 'Provisional');
                INSERT INTO [Expedientes].[dbo].[EstadosCalificaciones] (IdEstadoCalificacion, Nombre) VALUES (2, 'Definitiva');
                INSERT INTO [Expedientes].[dbo].[EstadosCalificaciones] (IdEstadoCalificacion, Nombre) VALUES (3, 'Cerrada');
                INSERT INTO [Expedientes].[dbo].[TiposConvocatorias] (IdTipoConvocatoria, Codigo, Nombre) VALUES (1, 'ORD', 'Ordinaria');
                INSERT INTO [Expedientes].[dbo].[TiposConvocatorias] (IdTipoConvocatoria, Codigo, Nombre) VALUES (2, 'EXT', 'Extraordinaria');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignaturasCalificaciones");

            migrationBuilder.DropTable(
                name: "EstadosCalificaciones");

            migrationBuilder.DropTable(
                name: "TiposConvocatorias");
        }
    }
}
