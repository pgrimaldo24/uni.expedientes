using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    internal class ConfiguracionExpedienteUniversidadEntityConfiguracion : IEntityTypeConfiguration<ConfiguracionExpedienteUniversidad>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionExpedienteUniversidad> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.IdRefUniversidad).HasMaxLength(50).IsRequired(false);
            builder.Property(r => r.AcronimoUniversidad).HasMaxLength(50).IsRequired(false);
            builder.Property(r => r.NombreUniversidad).HasMaxLength(100).IsRequired();
            builder.Property(r => r.IdIntegracionUniversidad).HasMaxLength(50).IsRequired();
            builder.Property(r => r.CodigoDocumental).HasMaxLength(50).IsRequired();
            builder.Property(r => r.TamanyoMaximoFichero).IsRequired();
            builder.Property(r => r.TiempoMaximoInactividad).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("ConfiguracionesExpedientesUniversidades");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();
        }
    }
}
