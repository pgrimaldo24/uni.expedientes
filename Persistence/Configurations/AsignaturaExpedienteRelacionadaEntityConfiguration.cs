using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class AsignaturaExpedienteRelacionadaEntityConfiguration : IEntityTypeConfiguration<AsignaturaExpedienteRelacionada>
    {
        public void Configure(EntityTypeBuilder<AsignaturaExpedienteRelacionada> builder)
        {
            builder.ToTable("AsignaturasExpedientesRelacionadas");
            builder.HasKey(ll => ll.Id);

            builder.HasOne(s => s.AsignaturaExpedienteOrigen)
                .WithMany(t => t.AsignaturasExpedientesOrigenes)
                .HasForeignKey(r => r.AsignaturaExpedienteOrigenId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.AsignaturaExpedienteDestino)
                .WithMany(t => t.AsignaturasExpedientesDestinos)
                .HasForeignKey(r => r.AsignaturaExpedienteDestinoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
