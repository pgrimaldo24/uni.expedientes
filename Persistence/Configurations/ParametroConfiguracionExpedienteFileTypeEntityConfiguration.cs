using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ParametroConfiguracionExpedienteFileTypeEntityConfiguration : IEntityTypeConfiguration<ParametroConfiguracionExpedienteFileType>
    {
        public void Configure(EntityTypeBuilder<ParametroConfiguracionExpedienteFileType> builder)
        {
            builder.ToTable("ParametrosConfiguracionesExpedientesFilesTypes");
            builder.Property(r => r.IdRefFileType).IsRequired();
            builder.Property(r => r.ParametroConfiguracionExpedienteId).IsRequired();

            //Relaciones a Uno
            builder.HasOne(s => s.ParametroConfiguracionExpediente)
                .WithMany(t => t.ParametrosConfiguracionesExpedientesFilesTypes)
                .HasForeignKey(t => t.ParametroConfiguracionExpedienteId).IsRequired();
        }
    }
}
