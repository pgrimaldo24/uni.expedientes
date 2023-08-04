using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RelacionExpedienteEntityConfiguration : IEntityTypeConfiguration<RelacionExpediente>
    {
        public void Configure(EntityTypeBuilder<RelacionExpediente> builder)
		{
            //Relaciones a Uno
            builder
                .HasOne(e => e.ExpedienteAlumno)
                .WithMany(e => e.RelacionesExpedientesOrigen)
                .HasForeignKey(e => e.ExpedienteAlumnoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(e => e.ExpedienteAlumnoRelacionado)
                .WithMany(e => e.RelacionesExpedientesRelacionadas)
                .HasForeignKey(e => e.ExpedienteAlumnoRelacionadoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.TipoRelacion)
                .WithMany(t => t.RelacionesExpedientes)
                .HasForeignKey(e => e.TipoRelacionId)
                .IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("RelacionesExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();
        }
	}
}
