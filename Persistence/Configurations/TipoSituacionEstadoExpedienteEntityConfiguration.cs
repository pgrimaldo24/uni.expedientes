using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoSituacionEstadoExpedienteEntityConfiguration : IEntityTypeConfiguration<TipoSituacionEstadoExpediente>
    {
        public void Configure(EntityTypeBuilder<TipoSituacionEstadoExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.FechaInicio).IsRequired();
            builder.Property(r => r.FechaFin).IsRequired(false);
            builder.Property(r => r.Descripcion).IsRequired(false);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TiposSituacionEstadoExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.TipoSituacionEstado).WithMany(t => t.TiposSituacionesEstadosExpedientes)
                .HasForeignKey(r => r.TipoSituacionEstadoId).IsRequired();
            builder.HasOne(e => e.ExpedienteAlumno).WithMany(e => e.TiposSituacionEstadoExpedientes)
                .HasForeignKey(r => r.ExpedienteAlumnoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
	}
}
