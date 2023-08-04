using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTable_AsignaturasExpedientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignaturasExpedientes");

            migrationBuilder.CreateTable(
                name: "AsignaturasExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefAsignaturaPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreAsignatura = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CodigoAsignatura = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrdenAsignatura = table.Column<int>(type: "int", nullable: false),
                    Ects = table.Column<double>(type: "float", nullable: false),
                    IdRefTipoAsignatura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SimboloTipoAsignatura = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OrdenTipoAsignatura = table.Column<int>(type: "int", nullable: false),
                    NombreTipoAsignatura = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdRefCurso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroCurso = table.Column<int>(type: "int", nullable: false),
                    AnyoAcademicoInicio = table.Column<int>(type: "int", nullable: false),
                    AnyoAcademicoFin = table.Column<int>(type: "int", nullable: false),
                    PeriodoLectivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DuracionPeriodo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SimboloDuracionPeriodo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IdRefIdiomaImparticion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SimboloIdiomaImparticion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Reconocida = table.Column<bool>(type: "bit", nullable: false),
                    SituacionAsignaturaId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false),
                    AsignaturaExpedienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturasExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientes_SituacionesAsignaturas_SituacionAsignaturaId",
                        column: x => x.SituacionAsignaturaId,
                        principalTable: "SituacionesAsignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientes_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientes_AsignaturasExpedientes_AsignaturaExpedienteId",
                        column: x => x.AsignaturaExpedienteId,
                        principalTable: "AsignaturasExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasExpedientes_AsignaturaExpedienteId",
                table: "AsignaturasExpedientes",
                column: "AsignaturaExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasExpedientes_ExpedienteAlumnoId",
                table: "AsignaturasExpedientes",
                column: "ExpedienteAlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturasExpedientes_SituacionAsignaturaId",
                table: "AsignaturasExpedientes",
                column: "SituacionAsignaturaId");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM[dbo].[SituacionesAsignaturas] WHERE Id = 1)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(1, 'Matriculada')
                IF NOT EXISTS(SELECT * FROM[dbo].[SituacionesAsignaturas] WHERE Id = 2)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(2, 'No Presentada')
                IF NOT EXISTS(SELECT * FROM[dbo].[SituacionesAsignaturas] WHERE Id = 3)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(3, 'No Superada')
                ELSE
	                UPDATE [SituacionesAsignaturas] SET Nombre = 'No Superada' WHERE Id = 3
                IF NOT EXISTS(SELECT * FROM[dbo].[SituacionesAsignaturas] WHERE Id = 4)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(4, 'Superada')
                ELSE
	                UPDATE [SituacionesAsignaturas] SET Nombre = 'Superada' WHERE Id = 4
                IF NOT EXISTS(SELECT * FROM[dbo].[SituacionesAsignaturas] WHERE Id = 5)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(5, 'Matrícula de Honor')
                IF NOT EXISTS(SELECT * FROM[dbo].[SituacionesAsignaturas] WHERE Id = 6)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(6, 'Anulada')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 7)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(7, 'Reconocida')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 8)
	                INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES(8, 'Adaptada')
            ");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "TiposSituacionEstado");

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "TiposSituacionEstado",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposSituacionEstado_EstadoId",
                table: "TiposSituacionEstado",
                column: "EstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TiposSituacionEstado_EstadosExpedientes_EstadoId",
                table: "TiposSituacionEstado",
                column: "EstadoId",
                principalTable: "EstadosExpedientes",
                principalColumn: "Id");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[TiposSituacionEstado] 
                VALUES 
                    ('Prematriculado', 1),
                    ('Cancelada desestimación de matrícula', 1),
                    ('Cancelada desestimación de ampliación de matrícula', 1),
                    ('Cancelada desestimación de variación de matrícula', 1),
                    ('Primera matrícula', 2),
                    ('Nueva matrícula', 2),
                    ('Cancelación de baja matrícula de nuevo ingreso', 2),
                    ('Ampliación matrícula', 2),
                    ('Cancelación de baja de ampliación de matrícula', 2),
                    ('Variación matrícula', 2),
                    ('Variación matrícula anulada', 2),
                    ('Variación matrícula recuperada', 2),
                    ('Reapertura de expediente', 2),
                    ('Matrícula desestimada', 3),
                    ('Baja matrícula de nuevo ingreso', 3),
                    ('Ampliación matrícula desestimada', 3),
                    ('Baja ampliación matrícula', 3),
                    ('Variación matrícula deja sin asignaturas', 3),
                    ('Variación matrícula desestimada', 3),
                    ('Variación matrícula anulada deja sin asignaturas', 3),
                    ('Variación matrícula recuperada deja sin asignaturas', 3),
                    ('Título expedido', 3),
                    ('Traslado de expediente', 3),
                    ('Superado tiempo de inactividad', 5),
                    ('Abandono', 5),
                    ('Impagado', 6),
                    ('Incumplimiento de normativa', 6),
                    ('Imposibilitado para expedir título', 6)
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignaturasExpedientes");

            migrationBuilder.DropForeignKey(
               name: "FK_TiposSituacionEstado_EstadosExpedientes_EstadoId",
               table: "TiposSituacionEstado");

            migrationBuilder.DropIndex(
                name: "IX_TiposSituacionEstado_EstadoId",
                table: "TiposSituacionEstado");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "TiposSituacionEstado");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "TiposSituacionEstado",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }
    }
}
