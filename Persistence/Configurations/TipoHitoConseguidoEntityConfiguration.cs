using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoHitoConseguidoEntityConfiguration : IEntityTypeConfiguration<TipoHitoConseguido>
    {
        public void Configure(EntityTypeBuilder<TipoHitoConseguido> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).HasMaxLength(250).IsRequired();
            builder.Property(r => r.Icono).HasMaxLength(50).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TiposHitoConseguidos");

            //Configuraci�n de Llave Primaria Autonum�rica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
	}
}
