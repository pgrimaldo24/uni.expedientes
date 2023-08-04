using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoRequisitoExpedienteEntityConfiguration : IEntityTypeConfiguration<TipoRequisitoExpediente>
    {
        public void Configure(EntityTypeBuilder<TipoRequisitoExpediente> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TiposRequisitosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
    }
}
