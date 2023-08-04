using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    public partial class AddTables_Requisitos_Comportamientos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComportamientosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstaVigente = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Bloqueado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComportamientosExpedientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosRequisitosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosRequisitosExpedientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequisitosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstaVigente = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequeridaParaTitulo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiereDocumentacion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnviarEmailAlumno = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequeridaParaPago = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EstaRestringida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsSancion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsLogro = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsCertificado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CertificadoIdioma = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiereTextoAdicional = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Bloqueado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IdRefModoRequerimientoDocumentacion = table.Column<int>(type: "int", nullable: true),
                    EstadoExpedienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientes_EstadosExpedientes_EstadoExpedienteId",
                        column: x => x.EstadoExpedienteId,
                        principalTable: "EstadosExpedientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TiposNivelesUso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposNivelesUso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposRequisitosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposRequisitosExpedientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CausasEstadosRequisitosConsolidadasExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EstadoRequisitoExpedienteId = table.Column<int>(type: "int", nullable: false),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CausasEstadosRequisitosConsolidadasExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CausasEstadosRequisitosConsolidadasExpedientes_EstadosRequisitosExpedientes_EstadoRequisitoExpedienteId",
                        column: x => x.EstadoRequisitoExpedienteId,
                        principalTable: "EstadosRequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CausasEstadosRequisitosConsolidadasExpedientes_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConsolidacionesRequisitosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EsDocumentacionFisica = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentacionRecibida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnviadaPorAlumno = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaCambioEstado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRefIdioma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SiglasIdioma = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NombreIdioma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NivelIdioma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoRequisitoExpedienteId = table.Column<int>(type: "int", nullable: true),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: false),
                    ExpedienteAlumnoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidacionesRequisitosExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsolidacionesRequisitosExpedientes_EstadosRequisitosExpedientes_EstadoRequisitoExpedienteId",
                        column: x => x.EstadoRequisitoExpedienteId,
                        principalTable: "EstadosRequisitosExpedientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsolidacionesRequisitosExpedientes_ExpedienteAlumno_ExpedienteAlumnoId",
                        column: x => x.ExpedienteAlumnoId,
                        principalTable: "ExpedienteAlumno",
                        principalColumn: "IdExpedienteAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsolidacionesRequisitosExpedientes_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitosExpedientesDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentoObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentoEditable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentoSecurizado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiereAceptacionAlumno = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IdRefPlantilla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosExpedientesDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientesDocumentos_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequisitosExpedientesFileType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdRefClasificacionDocumental = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosExpedientesFileType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientesFileType_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolesRequisitosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rol = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesRequisitosExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesRequisitosExpedientes_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NivelesUsoComportamientosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcronimoUniversidad = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IdRefTipoEstudio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreTipoEstudio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdRefEstudio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreEstudio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdRefPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombrePlan = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IdRefTipoAsignatura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreTipoAsignatura = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdRefAsignaturaPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdRefAsignatura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreAsignatura = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoNivelUsoId = table.Column<int>(type: "int", nullable: false),
                    ComportamientoExpedienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesUsoComportamientosExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NivelesUsoComportamientosExpedientes_ComportamientosExpedientes_ComportamientoExpedienteId",
                        column: x => x.ComportamientoExpedienteId,
                        principalTable: "ComportamientosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NivelesUsoComportamientosExpedientes_TiposNivelesUso_TipoNivelUsoId",
                        column: x => x.TipoNivelUsoId,
                        principalTable: "TiposNivelesUso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitosComportamientosExpedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComportamientoExpedienteId = table.Column<int>(type: "int", nullable: false),
                    RequisitoExpedienteId = table.Column<int>(type: "int", nullable: false),
                    TipoRequisitoExpedienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosComportamientosExpedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosComportamientosExpedientes_ComportamientosExpedientes_ComportamientoExpedienteId",
                        column: x => x.ComportamientoExpedienteId,
                        principalTable: "ComportamientosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequisitosComportamientosExpedientes_RequisitosExpedientes_RequisitoExpedienteId",
                        column: x => x.RequisitoExpedienteId,
                        principalTable: "RequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequisitosComportamientosExpedientes_TiposRequisitosExpedientes_TipoRequisitoExpedienteId",
                        column: x => x.TipoRequisitoExpedienteId,
                        principalTable: "TiposRequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsolidacionesRequisitosExpedientesDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fichero = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaFichero = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRefDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FicheroValidado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequisitoExpedienteDocumentoId = table.Column<int>(type: "int", nullable: true),
                    ConsolidacionRequisitoExpedienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidacionesRequisitosExpedientesDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsolidacionesRequisitosExpedientesDocumentos_ConsolidacionesRequisitosExpedientes_ConsolidacionRequisitoExpedienteId",
                        column: x => x.ConsolidacionRequisitoExpedienteId,
                        principalTable: "ConsolidacionesRequisitosExpedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsolidacionesRequisitosExpedientesDocumentos_RequisitosExpedientesDocumentos_RequisitoExpedienteDocumentoId",
                        column: x => x.RequisitoExpedienteDocumentoId,
                        principalTable: "RequisitosExpedientesDocumentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequisitosExpedientesDocumentosClasificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRefUniversidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdRefClasificacionDocumental = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequisitoExpedienteDocumentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosExpedientesDocumentosClasificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosExpedientesDocumentosClasificaciones_RequisitosExpedientesDocumentos_RequisitoExpedienteDocumentoId",
                        column: x => x.RequisitoExpedienteDocumentoId,
                        principalTable: "RequisitosExpedientesDocumentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CausasEstadosRequisitosConsolidadasExpedientes_EstadoRequisitoExpedienteId",
                table: "CausasEstadosRequisitosConsolidadasExpedientes",
                column: "EstadoRequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CausasEstadosRequisitosConsolidadasExpedientes_RequisitoExpedienteId",
                table: "CausasEstadosRequisitosConsolidadasExpedientes",
                column: "RequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_EstadoRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "EstadoRequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_ExpedienteAlumnoId",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "ExpedienteAlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientes_RequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientes",
                column: "RequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientesDocumentos_ConsolidacionRequisitoExpedienteId",
                table: "ConsolidacionesRequisitosExpedientesDocumentos",
                column: "ConsolidacionRequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidacionesRequisitosExpedientesDocumentos_RequisitoExpedienteDocumentoId",
                table: "ConsolidacionesRequisitosExpedientesDocumentos",
                column: "RequisitoExpedienteDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_NivelesUsoComportamientosExpedientes_ComportamientoExpedienteId",
                table: "NivelesUsoComportamientosExpedientes",
                column: "ComportamientoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_NivelesUsoComportamientosExpedientes_TipoNivelUsoId",
                table: "NivelesUsoComportamientosExpedientes",
                column: "TipoNivelUsoId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosComportamientosExpedientes_ComportamientoExpedienteId",
                table: "RequisitosComportamientosExpedientes",
                column: "ComportamientoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosComportamientosExpedientes_RequisitoExpedienteId",
                table: "RequisitosComportamientosExpedientes",
                column: "RequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosComportamientosExpedientes_TipoRequisitoExpedienteId",
                table: "RequisitosComportamientosExpedientes",
                column: "TipoRequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientes_EstadoExpedienteId",
                table: "RequisitosExpedientes",
                column: "EstadoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientesDocumentos_RequisitoExpedienteId",
                table: "RequisitosExpedientesDocumentos",
                column: "RequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientesDocumentosClasificaciones_RequisitoExpedienteDocumentoId",
                table: "RequisitosExpedientesDocumentosClasificaciones",
                column: "RequisitoExpedienteDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosExpedientesFileType_RequisitoExpedienteId",
                table: "RequisitosExpedientesFileType",
                column: "RequisitoExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesRequisitosExpedientes_RequisitoExpedienteId",
                table: "RolesRequisitosExpedientes",
                column: "RequisitoExpedienteId");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosRequisitosExpedientes] WHERE Id = 1)
                  INSERT INTO [dbo].[EstadosRequisitosExpedientes] ([Id], [Nombre], [Orden]) VALUES (1, 'No Procesado', 1)
                ELSE
                  UPDATE [dbo].[EstadosRequisitosExpedientes] SET Nombre = 'No Procesado', Orden = 1 WHERE Id = 1

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosRequisitosExpedientes] WHERE Id = 2)
                  INSERT INTO [dbo].[EstadosRequisitosExpedientes] ([Id], [Nombre], [Orden]) VALUES (2, 'Pendiente', 2)
                ELSE
                  UPDATE [dbo].[EstadosRequisitosExpedientes] SET Nombre = 'Pendiente', Orden = 2 WHERE Id = 2

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosRequisitosExpedientes] WHERE Id = 3)
                  INSERT INTO [dbo].[EstadosRequisitosExpedientes] ([Id], [Nombre], [Orden]) VALUES (3, 'Rechazado', 3)
                ELSE
                  UPDATE [dbo].[EstadosRequisitosExpedientes] SET Nombre = 'Rechazado', Orden = 3 WHERE Id = 3

                IF NOT EXISTS(SELECT * FROM [dbo].[EstadosRequisitosExpedientes] WHERE Id = 4)
                  INSERT INTO [dbo].[EstadosRequisitosExpedientes] ([Id], [Nombre], [Orden]) VALUES (4, 'Validado', 4)
                ELSE
                  UPDATE [dbo].[EstadosRequisitosExpedientes] SET Nombre = 'Validado', Orden = 4 WHERE Id = 4
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRequisitosExpedientes] WHERE Id = 1)
                  INSERT INTO [dbo].[TiposRequisitosExpedientes] ([Id], [Nombre]) VALUES (1, 'Obligatoria')
                ELSE
                  UPDATE [dbo].[TiposRequisitosExpedientes] SET Nombre = 'Obligatoria' WHERE Id = 1

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposRequisitosExpedientes] WHERE Id = 2)
                  INSERT INTO [dbo].[TiposRequisitosExpedientes] ([Id], [Nombre]) VALUES (2, 'Opcional')
                ELSE
                  UPDATE [dbo].[TiposRequisitosExpedientes] SET Nombre = 'Opcional' WHERE Id = 2
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM [dbo].[TiposNivelesUso] WHERE Id = 1)
                  INSERT INTO [dbo].[TiposNivelesUso] ([Id], [Nombre], [Orden]) VALUES (1, 'Universidad', 1)
                ELSE
                  UPDATE [dbo].[TiposNivelesUso] SET Nombre = 'Universidad', Orden = 1 WHERE Id = 1

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposNivelesUso] WHERE Id = 2)
                  INSERT INTO [dbo].[TiposNivelesUso] ([Id], [Nombre], [Orden]) VALUES (2, 'Tipo de Estudio', 2)
                ELSE
                  UPDATE [dbo].[TiposNivelesUso] SET Nombre = 'Tipo de Estudio', Orden = 2 WHERE Id = 2

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposNivelesUso] WHERE Id = 3)
                  INSERT INTO [dbo].[TiposNivelesUso] ([Id], [Nombre], [Orden]) VALUES (3, 'Estudio', 3)
                ELSE
                  UPDATE [dbo].[TiposNivelesUso] SET Nombre = 'Estudio', Orden = 3 WHERE Id = 3

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposNivelesUso] WHERE Id = 4)
                  INSERT INTO [dbo].[TiposNivelesUso] ([Id], [Nombre], [Orden]) VALUES (4, 'Plan de Estudio', 4)
                ELSE
                  UPDATE [dbo].[TiposNivelesUso] SET Nombre = 'Plan de Estudio', Orden = 4 WHERE Id = 4

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposNivelesUso] WHERE Id = 5)
                  INSERT INTO [dbo].[TiposNivelesUso] ([Id], [Nombre], [Orden]) VALUES (5, 'Tipo de Asignatura', 5)
                ELSE
                  UPDATE [dbo].[TiposNivelesUso] SET Nombre = 'Tipo de Asignatura', Orden = 5 WHERE Id = 5

                IF NOT EXISTS(SELECT * FROM [dbo].[TiposNivelesUso] WHERE Id = 6)
                  INSERT INTO [dbo].[TiposNivelesUso] ([Id], [Nombre], [Orden]) VALUES (6, 'Asignatura Plan', 6)
                ELSE
                  UPDATE [dbo].[TiposNivelesUso] SET Nombre = 'Asignatura Plan', Orden = 6 WHERE Id = 6
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CausasEstadosRequisitosConsolidadasExpedientes");

            migrationBuilder.DropTable(
                name: "ConsolidacionesRequisitosExpedientesDocumentos");

            migrationBuilder.DropTable(
                name: "NivelesUsoComportamientosExpedientes");

            migrationBuilder.DropTable(
                name: "RequisitosComportamientosExpedientes");

            migrationBuilder.DropTable(
                name: "RequisitosExpedientesDocumentosClasificaciones");

            migrationBuilder.DropTable(
                name: "RequisitosExpedientesFileType");

            migrationBuilder.DropTable(
                name: "RolesRequisitosExpedientes");

            migrationBuilder.DropTable(
                name: "ConsolidacionesRequisitosExpedientes");

            migrationBuilder.DropTable(
                name: "TiposNivelesUso");

            migrationBuilder.DropTable(
                name: "ComportamientosExpedientes");

            migrationBuilder.DropTable(
                name: "TiposRequisitosExpedientes");

            migrationBuilder.DropTable(
                name: "RequisitosExpedientesDocumentos");

            migrationBuilder.DropTable(
                name: "EstadosRequisitosExpedientes");

            migrationBuilder.DropTable(
                name: "RequisitosExpedientes");
        }
    }
}
