using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class AnotacionEntityConfiguracion : IEntityTypeConfiguration<Anotacion>
    {
        public void Configure(EntityTypeBuilder<Anotacion> builder)
        {
            builder.ToTable("Anotaciones");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Fecha)
                .IsRequired();
            builder.Property(e => e.IdRefCuentaSeguridad).IsRequired();
            builder.Property(e => e.EsPublica).IsRequired();
            builder.Property(e => e.EsRestringida).IsRequired();
            builder.Property(e => e.Resumen).IsRequired();
            builder.Property(e => e.FechaModificacion)
                .IsRequired();
            builder.Property(e => e.IdRefCuentaSeguridadModificacion)
                .IsRequired();
            builder
                .HasOne(e => e.ExpedienteAlumno)
                .WithMany(e => e.Anotaciones)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
