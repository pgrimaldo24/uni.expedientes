using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RequisitoComportamientoExpedienteEntityConfiguration : IEntityTypeConfiguration<RequisitoComportamientoExpediente>
    {
        public void Configure(EntityTypeBuilder<RequisitoComportamientoExpediente> builder)
		{
            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("RequisitosComportamientosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.ComportamientoExpediente).WithMany(t => t.RequisitosComportamientosExpedientes)
                .HasForeignKey(s => s.ComportamientoExpedienteId).IsRequired();
            builder.HasOne(s => s.RequisitoExpediente).WithMany(t => t.RequisitosComportamientosExpedientes)
                .HasForeignKey(s => s.RequisitoExpedienteId).IsRequired();
            builder.HasOne(s => s.TipoRequisitoExpediente).WithMany(t => t.RequisitosComportamientosExpedientes)
                .HasForeignKey(s => s.TipoRequisitoExpedienteId).IsRequired();
        }
	}
}
