using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoRelacionExpedienteEntityConfiguration : IEntityTypeConfiguration<TipoRelacionExpediente>
    {
        public void Configure(EntityTypeBuilder<TipoRelacionExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
            builder.Property(r => r.EsLogro).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TiposRelacionesExpediente");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
	}
}
