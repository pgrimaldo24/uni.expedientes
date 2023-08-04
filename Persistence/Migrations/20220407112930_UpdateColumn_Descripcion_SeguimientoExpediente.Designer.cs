﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Unir.Expedientes.Persistence.Context;

#nullable disable

namespace Unir.Expedientes.Persistence.Migrations
{
    [DbContext(typeof(ExpedientesContext))]
    [Migration("20220407112930_UpdateColumn_Descripcion_SeguimientoExpediente")]
    partial class UpdateColumn_Descripcion_SeguimientoExpediente
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ExpedienteAlumno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdExpedienteAlumno");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AcronimoUniversidad")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("AlumnoApellido1")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("AlumnoApellido2")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("AlumnoEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AlumnoNombre")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("AlumnoNroDocIdentificacion")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DocAcreditativoViaAcceso")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FechaApertura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaExpedicion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaFinalizacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaPago")
                        .HasColumnType("datetime2");

                    b.Property<string>("FechaSubidaDocViaAcceso")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("FechaTrabajoFinEstudio")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdRefAlumno")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefAreaAcademica")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefCentro")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefEstudio")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefIntegracionAlumno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefIntegracionDocViaAcceso")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefNodo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefPlan")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefTipoDocumentoIdentificacionPais")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefTipoEstudio")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefTipoVinculacion")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefTitulo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefUniversidad")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefVersionPlan")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IdRefViaAccesoPlan")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("IdTitulacionAcceso")
                        .HasColumnType("int")
                        .HasColumnName("TitulacionAccesoId");

                    b.Property<string>("NombreEstudio")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("NombrePlan")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<double?>("NotaMedia")
                        .HasColumnType("float");

                    b.Property<string>("TituloTrabajoFinEstudio")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.HasKey("Id");

                    b.HasIndex("IdTitulacionAcceso");

                    b.ToTable("ExpedienteAlumno", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ExpedienteEspecializacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ExpedienteAlumnoId")
                        .HasColumnType("int");

                    b.Property<string>("IdRefEspecializacion")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExpedienteAlumnoId", "IdRefEspecializacion");

                    b.ToTable("ExpedientesEspecializaciones", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ParametroConfiguracionExpediente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("ParametrosConfiguracionesExpedientes", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ParametroConfiguracionExpedienteFileType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("IdRefFileType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ParametroConfiguracionExpedienteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParametroConfiguracionExpedienteId");

                    b.ToTable("ParametrosConfiguracionesExpedientesFilesTypes", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.SeguimientoExpediente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdSeguimientoExpediente");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExpedienteAlumnoId")
                        .HasColumnType("int")
                        .HasColumnName("IdExpedienteAlumno");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdCuentaSeguridad")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("IdRefTrabajador")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("OrigenExterno")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("TipoId")
                        .HasColumnType("int")
                        .HasColumnName("IdTipoSeguimientoExpediente");

                    b.HasKey("Id");

                    b.HasIndex("ExpedienteAlumnoId");

                    b.HasIndex("TipoId");

                    b.ToTable("SeguimientoExpediente", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.TipoSeguimientoExpediente", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("IdTipoSeguimientoExpediente");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("TipoSeguimientoExpediente", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.TitulacionAcceso", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CodigoColegiadoProfesional")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("FechaInicioTitulo")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechafinTitulo")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdRefTerritorioInstitucionDocente")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("InstitucionDocente")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int?>("NroSemestreRealizados")
                        .HasColumnType("int");

                    b.Property<string>("TipoEstudio")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("TitulacionesAccesos", (string)null);
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ExpedienteAlumno", b =>
                {
                    b.HasOne("Unir.Expedientes.Domain.Entities.TitulacionAcceso", "TitulacionAcceso")
                        .WithMany("ExpedientesAlumnos")
                        .HasForeignKey("IdTitulacionAcceso");

                    b.Navigation("TitulacionAcceso");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ExpedienteEspecializacion", b =>
                {
                    b.HasOne("Unir.Expedientes.Domain.Entities.ExpedienteAlumno", "ExpedienteAlumno")
                        .WithMany("ExpedientesEspecializaciones")
                        .HasForeignKey("ExpedienteAlumnoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExpedienteAlumno");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ParametroConfiguracionExpedienteFileType", b =>
                {
                    b.HasOne("Unir.Expedientes.Domain.Entities.ParametroConfiguracionExpediente", "ParametroConfiguracionExpediente")
                        .WithMany("ParametrosConfiguracionesExpedientesFilesTypes")
                        .HasForeignKey("ParametroConfiguracionExpedienteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParametroConfiguracionExpediente");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.SeguimientoExpediente", b =>
                {
                    b.HasOne("Unir.Expedientes.Domain.Entities.ExpedienteAlumno", "ExpedienteAlumno")
                        .WithMany("Seguimientos")
                        .HasForeignKey("ExpedienteAlumnoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Unir.Expedientes.Domain.Entities.TipoSeguimientoExpediente", "Tipo")
                        .WithMany("Seguimientos")
                        .HasForeignKey("TipoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExpedienteAlumno");

                    b.Navigation("Tipo");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ExpedienteAlumno", b =>
                {
                    b.Navigation("ExpedientesEspecializaciones");

                    b.Navigation("Seguimientos");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.ParametroConfiguracionExpediente", b =>
                {
                    b.Navigation("ParametrosConfiguracionesExpedientesFilesTypes");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.TipoSeguimientoExpediente", b =>
                {
                    b.Navigation("Seguimientos");
                });

            modelBuilder.Entity("Unir.Expedientes.Domain.Entities.TitulacionAcceso", b =>
                {
                    b.Navigation("ExpedientesAlumnos");
                });
#pragma warning restore 612, 618
        }
    }
}
