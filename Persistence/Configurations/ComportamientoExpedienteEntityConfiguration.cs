using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ComportamientoExpedienteEntityConfiguration : IEntityTypeConfiguration<ComportamientoExpediente>
    {
        public void Configure(EntityTypeBuilder<ComportamientoExpediente> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();
            builder.Property(r => r.Descripcion).IsRequired(false);
            builder.Property(r => r.EstaVigente).HasDefaultValue(true).IsRequired();
            builder.Property(r => r.Bloqueado).HasDefaultValue(false).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("ComportamientosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();
        }
    }
}
