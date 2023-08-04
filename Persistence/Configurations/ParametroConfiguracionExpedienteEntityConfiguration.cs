using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ParametroConfiguracionExpedienteEntityConfiguration : IEntityTypeConfiguration<ParametroConfiguracionExpediente>
    {
        public void Configure(EntityTypeBuilder<ParametroConfiguracionExpediente> builder)
        {
            builder.ToTable("ParametrosConfiguracionesExpedientes");
            builder.Property(r => r.Nombre).IsRequired();
            builder
                .HasMany(e => e.ParametrosConfiguracionesExpedientesFilesTypes)
                .WithOne(s => s.ParametroConfiguracionExpediente)
                .HasForeignKey(i => i.ParametroConfiguracionExpedienteId);
        }
    }
}
