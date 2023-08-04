using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class AsignaturaExpedienteEntityConfiguration : IEntityTypeConfiguration<AsignaturaExpediente>
    {
        public void Configure(EntityTypeBuilder<AsignaturaExpediente> builder)
        {
            builder.ToTable("AsignaturasExpedientes");
            builder.HasKey(ae => ae.Id);
            builder.Property(ae => ae.Id).ValueGeneratedOnAdd();
            builder.Property(ae => ae.IdRefAsignaturaPlan).IsRequired(false).HasMaxLength(50);
            builder.Property(ae => ae.NombreAsignatura).IsRequired().HasMaxLength(250);
            builder.Property(ae => ae.CodigoAsignatura).IsRequired().HasMaxLength(100);
            builder.Property(ae => ae.OrdenAsignatura).IsRequired();
            builder.Property(ae => ae.Ects).IsRequired();
            builder.Property(ae => ae.IdRefTipoAsignatura).IsRequired().HasMaxLength(50);
            builder.Property(ae => ae.SimboloTipoAsignatura).IsRequired().HasMaxLength(25);
            builder.Property(ae => ae.OrdenTipoAsignatura).IsRequired();
            builder.Property(ae => ae.NombreTipoAsignatura).IsRequired().HasMaxLength(200);
            builder.Property(ae => ae.IdRefCurso).IsRequired().HasMaxLength(50);
            builder.Property(ae => ae.NumeroCurso).IsRequired();
            builder.Property(ae => ae.AnyoAcademicoInicio).IsRequired();
            builder.Property(ae => ae.AnyoAcademicoFin).IsRequired();
            builder.Property(ae => ae.PeriodoLectivo).IsRequired().HasMaxLength(150);
            builder.Property(ae => ae.DuracionPeriodo).IsRequired().HasMaxLength(200);
            builder.Property(ae => ae.SimboloDuracionPeriodo).IsRequired().HasMaxLength(10);
            builder.Property(ae => ae.IdRefIdiomaImparticion).IsRequired().HasMaxLength(50);
            builder.Property(ae => ae.SimboloIdiomaImparticion).IsRequired().HasMaxLength(10);
            builder.Property(ae => ae.Reconocida).IsRequired();

            builder.HasOne(s => s.SituacionAsignatura).WithMany(t => t.AsignaturasExpedientes)
                .HasForeignKey(r => r.SituacionAsignaturaId).IsRequired();
            builder.HasOne(s => s.ExpedienteAlumno).WithMany(t => t.AsignaturasExpedientes)
                .HasForeignKey(r => r.ExpedienteAlumnoId).IsRequired();

            builder
                .HasMany(e => e.AsignaturasCalificaciones)
                .WithOne(s => s.AsignaturaExpediente)
                .HasForeignKey(i => i.AsignaturaExpedienteId);
        }
    }
}