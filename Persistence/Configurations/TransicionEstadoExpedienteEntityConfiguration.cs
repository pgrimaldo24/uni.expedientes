using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TransicionEstadoExpedienteEntityConfiguration : IEntityTypeConfiguration<TransicionEstadoExpediente>
    {
        public void Configure(EntityTypeBuilder<TransicionEstadoExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.EsInversa).IsRequired();
            builder.Property(r => r.EsManual).IsRequired();
            builder.Property(r => r.Nombre).IsRequired(false);
            builder.Property(r => r.Orden).IsRequired(false);
            builder.Property(r => r.SoloAdmin).IsRequired();

            //Relaciones a Uno
            builder
                .HasOne(e => e.EstadoOrigen)
                .WithMany(e => e.TransicionesOrigen)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(e => e.EstadoDestino)
                .WithMany(e => e.TransicionesDestino)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TransicionesEstadosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
	}
}
