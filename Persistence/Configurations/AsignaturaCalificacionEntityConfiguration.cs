using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class AsignaturaCalificacionEntityConfiguration : IEntityTypeConfiguration<AsignaturaCalificacion>
    {
        public void Configure(EntityTypeBuilder<AsignaturaCalificacion> builder)
        {
            builder.ToTable("AsignaturasCalificaciones");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("IdAsignaturaCalificacion");
            builder.Property(r => r.FechaPublicado).IsRequired(false);
            builder.Property(r => r.FechaConfirmado).IsRequired(false);
            builder.Property(r => r.IdRefPeriodoLectivo).IsRequired();
            builder.Property(r => r.Ciclo).IsRequired();
            builder.Property(r => r.AnyoAcademico).IsRequired();
            builder.Property(r => r.Calificacion).IsRequired(false);
            builder.Property(r => r.NombreCalificacion).IsRequired(false);
            builder.Property(r => r.Convocatoria).IsRequired();
            builder.Property(r => r.IdRefAsignaturaMatriculada).IsRequired(false);
            builder.Property(r => r.IdRefAsignaturaOfertada).IsRequired(false);
            builder.Property(r => r.Plataforma).IsRequired(false);
            builder.Property(r => r.IdRefGrupoCurso).IsRequired(false);
            builder.Property(r => r.IdPublicadorConfirmador).IsRequired(false);
            builder.Property(r => r.EsMatriculaHonor).IsRequired();
            builder.Property(r => r.EsNoPresentado).IsRequired();
            builder.Property(r => r.Superada).IsRequired();

            //Relaciones a Uno
            builder.HasOne(r => r.AsignaturaExpediente).WithMany(t => t.AsignaturasCalificaciones).HasForeignKey(r => r.AsignaturaExpedienteId);
            builder.HasOne(r => r.TipoConvocatoria).WithMany(t => t.AsignaturasCalificaciones).HasForeignKey(r => r.TipoConvocatoriaId);
            builder.HasOne(r => r.EstadoCalificacion).WithMany(t => t.AsignaturasCalificaciones).HasForeignKey(r => r.EstadoCalificacionId);
        }

    }
}
