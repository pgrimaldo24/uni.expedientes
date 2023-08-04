using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTables_AsignaturasExpedientes_SituacionesAsignaturas_AddColumn_ExpedienteAlumnoId_Table_TiposSituacionEstadoExpedientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpedienteAlumnoId",
                table: "TiposSituacionEstadoExpedientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SituacionesAsignaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacionesAsignaturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AsignaturasExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefAsignaturaPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreAsignatura = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Ects = table.Column<double>(type: "float", nullable: false),
                    IdRefTipoAsignatura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SimboloTipoAsignatura = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    NombreIdioma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AsignaturaExpedienteId = table.Column<int>(type: "int", nullable: true),
                    SituacionAsignaturaId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturasExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientes_AsignaturasExpedientes_AsignaturaExpedienteId",
                        column: x => x.AsignaturaExpedienteId,
                        principalTable: "AsignaturasExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientes_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignaturasExpedientes_SituacionesAsignaturas_SituacionAsignaturaId",
                        column: x => x.SituacionAsignaturaId,
                        principalTable: "SituacionesAsignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TiposSituacionEstadoExpedientes_ExpedienteAlumnoId",
                table: "TiposSituacionEstadoExpedientes",
                column: "ExpedienteAlumnoId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TiposSituacionEstadoExpedientes_ExpedienteAlumno_ExpedienteAlumnoId",
                table: "TiposSituacionEstadoExpedientes",
                column: "ExpedienteAlumnoId",
                principalTable: "ExpedienteAlumno",
                principalColumn: "IdExpedienteAlumno",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 1)
                  INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES (1,'Matriculada')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 2)
                  INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES (2,'No Presentada')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 3)
                  INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES (3,'Suspensa')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 4)
                  INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES (4,'Aprobada')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 5)
                  INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES (5,'Matrícula de Honor')
                IF NOT EXISTS(SELECT * FROM [dbo].[SituacionesAsignaturas] WHERE Id = 6)
                  INSERT INTO [dbo].[SituacionesAsignaturas] ([Id],[Nombre]) VALUES (6,'Anulada')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiposSituacionEstadoExpedientes_ExpedienteAlumno_ExpedienteAlumnoId",
                table: "TiposSituacionEstadoExpedientes");

            migrationBuilder.DropTable(
                name: "AsignaturasExpedientes");

            migrationBuilder.DropTable(
                name: "SituacionesAsignaturas");

            migrationBuilder.DropIndex(
                name: "IX_TiposSituacionEstadoExpedientes_ExpedienteAlumnoId",
                table: "TiposSituacionEstadoExpedientes");

            migrationBuilder.DropColumn(
                name: "ExpedienteAlumnoId",
                table: "TiposSituacionEstadoExpedientes");
        }
    }
}
