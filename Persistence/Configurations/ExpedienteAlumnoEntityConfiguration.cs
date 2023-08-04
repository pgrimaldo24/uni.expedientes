using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ExpedienteAlumnoEntityConfiguration : IEntityTypeConfiguration<ExpedienteAlumno>
	{
        public void Configure(EntityTypeBuilder<ExpedienteAlumno> builder)
        {
            builder.ToTable("ExpedienteAlumno");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("IdExpedienteAlumno");
            builder.Property(e => e.IdTitulacionAcceso).HasColumnName("TitulacionAccesoId");
            builder.Property(r => r.IdRefVersionPlan).IsRequired(false);
            builder.Property(r => r.IdRefNodo).IsRequired(false);
            builder.Property(r => r.AlumnoNombre).IsRequired(false);
            builder.Property(r => r.AlumnoApellido1).IsRequired(false);
            builder.Property(r => r.AlumnoApellido2).IsRequired(false);
            builder.Property(r => r.AlumnoNroDocIdentificacion).IsRequired(false);
            builder.Property(r => r.AlumnoEmail).IsRequired(false);
            builder.Property(r => r.NombrePlan).IsRequired(false);
            builder.Property(r => r.NombreEstudio).IsRequired(false);
            builder.Property(r => r.AcronimoUniversidad).IsRequired(false);
            builder.Property(r => r.DocAcreditativoViaAcceso).IsRequired(false);
            builder.Property(r => r.FechaApertura).IsRequired(false);
            builder.Property(r => r.FechaSubidaDocViaAcceso).IsRequired(false);
            builder.Property(r => r.FechaFinalizacion).IsRequired(false);
            builder.Property(r => r.FechaTrabajoFinEstudio).IsRequired(false);
            builder.Property(r => r.TituloTrabajoFinEstudio).IsRequired(false);
            builder.Property(r => r.NotaMedia).IsRequired(false);
            builder.Property(r => r.FechaPago).IsRequired(false);
            builder.Property(e => e.Migrado).IsRequired();
            builder
                .HasMany(e => e.Seguimientos)
                .WithOne(s => s.ExpedienteAlumno)
                .HasForeignKey(i => i.ExpedienteAlumnoId);

            builder
                .HasMany(e => e.ExpedientesEspecializaciones)
                .WithOne(s => s.ExpedienteAlumno)
                .HasForeignKey(i => i.ExpedienteAlumnoId);

            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.IdRefIntegracionAlumno).IsRequired();
            builder.Property(r => r.IdRefPlan).IsRequired();
            builder.Property(r => r.IdRefTipoDocumentoIdentificacionPais).IsRequired(false);
            builder.Property(r => r.IdRefUniversidad).IsRequired(false);
            builder.Property(r => r.IdRefCentro).IsRequired(false);
            builder.Property(r => r.IdRefAreaAcademica).IsRequired(false);
            builder.Property(r => r.IdRefTipoEstudio).IsRequired(false);
            builder.Property(r => r.IdRefEstudio).IsRequired(false);
            builder.Property(r => r.IdRefTitulo).IsRequired(false);
            builder.Property(r => r.IdRefViaAccesoPlan).IsRequired(false);
            builder.Property(r => r.IdRefIntegracionDocViaAcceso).IsRequired(false);
            builder.Property(r => r.IdRefTipoVinculacion).IsRequired(false);

            //Relaciones a Uno
            builder.HasOne(r => r.TitulacionAcceso).WithMany(t => t.ExpedientesAlumnos).HasForeignKey(r => r.IdTitulacionAcceso);
            builder.HasOne(r => r.Estado).WithMany(t => t.ExpedientesAlumnos).HasForeignKey(r => r.EstadoId);
        }
    }
}
