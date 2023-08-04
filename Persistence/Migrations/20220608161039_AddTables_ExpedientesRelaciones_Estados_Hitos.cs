using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTables_ExpedientesRelaciones_Estados_Hitos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "SeguimientoExpediente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "ExpedienteAlumno",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EstadosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosExpedientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposHitoConseguidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposHitoConseguidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposRelacionesExpediente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EsLogro = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposRelacionesExpediente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposSituacionEstado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSituacionEstado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransicionesEstadosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EsInversa = table.Column<bool>(type: "bit", nullable: false),
                    EsManual = table.Column<bool>(type: "bit", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: true),
                    SoloAdmin = table.Column<bool>(type: "bit", nullable: false),
                    EstadoOrigenId = table.Column<int>(type: "int", nullable: false),
                    EstadoDestinoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransicionesEstadosExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransicionesEstadosExpedientes_EstadosExpedientes_EstadoDestinoId",
                        column: x => x.EstadoDestinoId,
                        principalTable: "EstadosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicionesEstadosExpedientes_EstadosExpedientes_EstadoOrigenId",
                        column: x => x.EstadoOrigenId,
                        principalTable: "EstadosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HitosConseguidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoConseguidoId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteEspecializacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitosConseguidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitosConseguidos_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HitosConseguidos_ExpedientesEspecializaciones_ExpedienteEspecializacionId",
                        column: x => x.ExpedienteEspecializacionId,
                        principalTable: "ExpedientesEspecializaciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HitosConseguidos_TiposHitoConseguidos_TipoConseguidoId",
                        column: x => x.TipoConseguidoId,
                        principalTable: "TiposHitoConseguidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelacionesExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteAlumnoRelacionadoId = table.Column<int>(type: "int", nullable: false),
                    TipoRelacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelacionesExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelacionesExpedientes_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelacionesExpedientes_ExpedienteAlumno_ExpedienteAlumnoRelacionadoId",
                        column: x => x.ExpedienteAlumnoRelacionadoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelacionesExpedientes_TiposRelacionesExpediente_TipoRelacionId",
                        column: x => x.TipoRelacionId,
                        principalTable: "TiposRelacionesExpediente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposSituacionEstadoExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoSituacionEstadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSituacionEstadoExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposSituacionEstadoExpedientes_TiposSituacionEstado_TipoSituacionEstadoId",
                        column: x => x.TipoSituacionEstadoId,
                        principalTable: "TiposSituacionEstado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeguimientoExpediente_EstadoId",
                table: "SeguimientoExpediente",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpedienteAlumno_EstadoId",
                table: "ExpedienteAlumno",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_HitosConseguidos_ExpedienteAlumnoId",
                table: "HitosConseguidos",
                column: "ExpedienteAlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_HitosConseguidos_ExpedienteEspecializacionId",
                table: "HitosConseguidos",
                column: "ExpedienteEspecializacionId");

            migrationBuilder.CreateIndex(
                name: "IX_HitosConseguidos_TipoConseguidoId",
                table: "HitosConseguidos",
                column: "TipoConseguidoId");

            migrationBuilder.CreateIndex(
                name: "IX_RelacionesExpedientes_ExpedienteAlumnoId",
                table: "RelacionesExpedientes",
                column: "ExpedienteAlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_RelacionesExpedientes_ExpedienteAlumnoRelacionadoId",
                table: "RelacionesExpedientes",
                column: "ExpedienteAlumnoRelacionadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RelacionesExpedientes_TipoRelacionId",
                table: "RelacionesExpedientes",
                column: "TipoRelacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposSituacionEstadoExpedientes_TipoSituacionEstadoId",
                table: "TiposSituacionEstadoExpedientes",
                column: "TipoSituacionEstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstadosExpedientes_EstadoDestinoId",
                table: "TransicionesEstadosExpedientes",
                column: "EstadoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstadosExpedientes_EstadoOrigenId",
                table: "TransicionesEstadosExpedientes",
                column: "EstadoOrigenId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteAlumno_EstadosExpedientes_EstadoId",
                table: "ExpedienteAlumno",
                column: "EstadoId",
                principalTable: "EstadosExpedientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeguimientoExpediente_EstadosExpedientes_EstadoId",
                table: "SeguimientoExpediente",
                column: "EstadoId",
                principalTable: "EstadosExpedientes",
                principalColumn: "Id");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosExpedientes] WHERE Id = 1)
                  INSERT INTO [dbo].[EstadosExpedientes] ([Id], [Nombre]) VALUES (1, 'Inicial')
                ELSE
                  UPDATE [dbo].[EstadosExpedientes] SET Nombre = 'Inicial' WHERE Id = 1

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosExpedientes] WHERE Id = 2)
                  INSERT INTO [dbo].[EstadosExpedientes] ([Id], [Nombre]) VALUES (2, 'Abierto')
                ELSE
                  UPDATE [dbo].[EstadosExpedientes] SET Nombre = 'Abierto' WHERE Id = 2

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosExpedientes] WHERE Id = 3)
                  INSERT INTO [dbo].[EstadosExpedientes] ([Id], [Nombre]) VALUES (3, 'Cerrado')
                ELSE
                  UPDATE [dbo].[EstadosExpedientes] SET Nombre = 'Cerrado' WHERE Id = 3

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosExpedientes] WHERE Id = 4)
                  INSERT INTO [dbo].[EstadosExpedientes] ([Id], [Nombre]) VALUES (4, 'Anulado')
                ELSE
                  UPDATE [dbo].[EstadosExpedientes] SET Nombre = 'Anulado' WHERE Id = 4

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosExpedientes] WHERE Id = 5)
                  INSERT INTO [dbo].[EstadosExpedientes] ([Id], [Nombre]) VALUES (5, 'Inactivo')
                ELSE
                  UPDATE [dbo].[EstadosExpedientes] SET Nombre = 'Inactivo' WHERE Id = 5

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosExpedientes] WHERE Id = 6)
                  INSERT INTO [dbo].[EstadosExpedientes] ([Id], [Nombre]) VALUES (6, 'Bloqueado')
                ELSE
                  UPDATE [dbo].[EstadosExpedientes] SET Nombre = 'Bloqueado' WHERE Id = 6
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 1)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (1, 'n-Titulacion', 0)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'n-Titulacion' WHERE Id = 1

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 2)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (2, 'Cambio plan', 0)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'Cambio plan' WHERE Id = 2

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 3)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (3, 'Seminario', 1)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'Seminario' WHERE Id = 3

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 4)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (4, 'Opción de Titulación', 1)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'Opción de Titulación' WHERE Id = 4

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 5)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (5, 'Servicios Sociales', 1)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'Servicios Sociales' WHERE Id = 5

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 6)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (6, 'Cursos Adaptación', 0)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'Cursos Adaptación' WHERE Id = 6

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRelacionesExpediente] WHERE Id = 7)
                  INSERT INTO [dbo].[TiposRelacionesExpediente] ([Id], [Nombre], [EsLogro]) VALUES (7, 'Cursos Cualificación', 0)
                ELSE
                  UPDATE [dbo].[TiposRelacionesExpediente] SET Nombre = 'Cursos Cualificación' WHERE Id = 7
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 1)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (1, 'Prematriculado')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Prematriculado' WHERE Id = 1

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 2)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (2, 'Primera matrícula')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Primera matrícula' WHERE Id = 2

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 3)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (3, 'Matriculado')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Matriculado' WHERE Id = 3

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 4)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (4, 'Inactivo')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Inactivo' WHERE Id = 4

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 5)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (5, 'Bloqueado')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Bloqueado' WHERE Id = 5

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 6)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (6, 'Cerrado')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Cerrado' WHERE Id = 6

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 7)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (7, 'Reabierto')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Reabierto' WHERE Id = 7

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 8)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (8, 'Trabajo Fin Estudio')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Trabajo Fin Estudio' WHERE Id = 8

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 9)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (9, 'Especialización')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Especialización' WHERE Id = 9

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 10)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (10, 'Tasas Abonadas')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Tasas Abonadas' WHERE Id = 10

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 11)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (11, 'Sancionado')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Sancionado' WHERE Id = 11

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposHitoConseguidos] WHERE Id = 12)
                  INSERT INTO [dbo].[TiposHitoConseguidos] ([Id], [Nombre]) VALUES (12, 'Premiado')
                ELSE
                  UPDATE [dbo].[TiposHitoConseguidos] SET Nombre = 'Premiado' WHERE Id = 12
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 1 AND EstadoDestinoId = 2)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (1,0,0,null,null,0,1,2)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 2 AND EstadoDestinoId = 3)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (2,0,0,null,null,0,2,3)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 2 AND EstadoDestinoId = 4)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (3,0,0,null,null,0,2,4)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 2 AND EstadoDestinoId = 5)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (4,0,0,null,null,0,2,5)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 2 AND EstadoDestinoId = 6)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (5,0,0,null,null,0,2,6)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 3 AND EstadoDestinoId = 2)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (6,1,0,null,null,0,3,2)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 5 AND EstadoDestinoId = 2)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (7,1,0,null,null,0,5,2)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 5 AND EstadoDestinoId = 3)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (8,0,0,null,null,0,5,3)
                IF NOT EXISTS(SELECT * FROM [dbo].[TransicionesEstadosExpedientes] WHERE EstadoOrigenId = 6 AND EstadoDestinoId = 3)
                  INSERT INTO [dbo].[TransicionesEstadosExpedientes] ([Id],[EsInversa],[EsManual],[Nombre],[Orden],[SoloAdmin],[EstadoOrigenId],[EstadoDestinoId]) VALUES (9,0,0,null,null,0,6,3)
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TipoSeguimientoExpediente] WHERE IdTipoSeguimientoExpediente = 6)
	                INSERT INTO [dbo].[TipoSeguimientoExpediente] ([IdTipoSeguimientoExpediente], [Nombre]) VALUES (6, 'Expediente actualizado en proceso masivo')
                ELSE
                    UPDATE [dbo].[TipoSeguimientoExpediente] SET [Nombre] = 'Expediente actualizado en proceso masivo' WHERE [IdTipoSeguimientoExpediente] = 6
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteAlumno_EstadosExpedientes_EstadoId",
                table: "ExpedienteAlumno");

            migrationBuilder.DropForeignKey(
                name: "FK_SeguimientoExpediente_EstadosExpedientes_EstadoId",
                table: "SeguimientoExpediente");

            migrationBuilder.DropTable(
                name: "HitosConseguidos");

            migrationBuilder.DropTable(
                name: "RelacionesExpedientes");

            migrationBuilder.DropTable(
                name: "TiposSituacionEstadoExpedientes");

            migrationBuilder.DropTable(
                name: "TransicionesEstadosExpedientes");

            migrationBuilder.DropTable(
                name: "TiposHitoConseguidos");

            migrationBuilder.DropTable(
                name: "TiposRelacionesExpediente");

            migrationBuilder.DropTable(
                name: "TiposSituacionEstado");

            migrationBuilder.DropTable(
                name: "EstadosExpedientes");

            migrationBuilder.DropIndex(
                name: "IX_SeguimientoExpediente_EstadoId",
                table: "SeguimientoExpediente");

            migrationBuilder.DropIndex(
                name: "IX_ExpedienteAlumno_EstadoId",
                table: "ExpedienteAlumno");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "SeguimientoExpediente");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "ExpedienteAlumno");
        }
    }
}
