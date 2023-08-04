using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class EstadoRequisitoExpedienteEntityConfiguration : IEntityTypeConfiguration<EstadoRequisitoExpediente>
    {
        public void Configure(EntityTypeBuilder<EstadoRequisitoExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();
            builder.Property(r => r.Orden).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("EstadosRequisitosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
	}
}
