using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoNivelUsoEntityConfiguration : IEntityTypeConfiguration<TipoNivelUso>
    {
        public void Configure(EntityTypeBuilder<TipoNivelUso> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();
            builder.Property(r => r.Orden).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TiposNivelesUso");

            //Configuraci�n de Llave Primaria Autonum�rica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
	}
}
