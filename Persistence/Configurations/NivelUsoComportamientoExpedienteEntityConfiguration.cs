using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class NivelUsoComportamientoExpedienteEntityConfiguration : IEntityTypeConfiguration<NivelUsoComportamientoExpediente>
    {
        public void Configure(EntityTypeBuilder<NivelUsoComportamientoExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.IdRefUniversidad).IsRequired();
            builder.Property(r => r.AcronimoUniversidad).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("NivelesUsoComportamientosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.TipoNivelUso).WithMany(t => t.NivelesUsoComportamientosExpedientes)
                .HasForeignKey(s => s.TipoNivelUsoId).IsRequired();
            builder.HasOne(s => s.ComportamientoExpediente).WithMany(t => t.NivelesUsoComportamientosExpedientes)
                .HasForeignKey(s => s.ComportamientoExpedienteId).IsRequired();
        }
	}
}
