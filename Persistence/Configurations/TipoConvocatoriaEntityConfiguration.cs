using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    internal class TipoConvocatoriaEntityConfiguration : IEntityTypeConfiguration<TipoConvocatoria>
    {
        public void Configure(EntityTypeBuilder<TipoConvocatoria> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Codigo).IsRequired();
            builder.Property(r => r.Nombre).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.Property(e => e.Id).HasColumnName("IdTipoConvocatoria");
            builder.ToTable("TiposConvocatorias");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
    }
}
