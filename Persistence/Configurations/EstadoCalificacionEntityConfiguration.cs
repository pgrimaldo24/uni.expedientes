using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class EstadoCalificacionEntityConfiguration : IEntityTypeConfiguration<EstadoCalificacion>
    {
        public void Configure(EntityTypeBuilder<EstadoCalificacion> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.Property(e => e.Id).HasColumnName("IdEstadoCalificacion");
            builder.ToTable("EstadosCalificaciones");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
    }
}
