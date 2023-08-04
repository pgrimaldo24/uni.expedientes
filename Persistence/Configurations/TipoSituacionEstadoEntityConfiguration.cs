using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoSituacionEstadoEntityConfiguration : IEntityTypeConfiguration<TipoSituacionEstado>
    {
        public void Configure(EntityTypeBuilder<TipoSituacionEstado> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired().HasMaxLength(100);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("TiposSituacionEstado");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.Estado).WithMany(t => t.TiposSituacionEstado);
        }
	}
}
