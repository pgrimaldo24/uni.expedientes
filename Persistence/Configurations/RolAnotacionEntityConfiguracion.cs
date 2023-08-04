using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RolAnotacionEntityConfiguracion : IEntityTypeConfiguration<RolAnotacion>
    {
        public void Configure(EntityTypeBuilder<RolAnotacion> builder)
        {
            builder.ToTable("RolesAnotaciones");
            builder.HasKey(ra => ra.Id);
            builder.HasAlternateKey(ra => new { ra.AnotacionId, ra.Rol });
            builder
                .Property(ra => ra.Rol)
                .IsRequired();
            builder
                .HasOne(ra => ra.Anotacion)
                .WithMany(ra => ra.RolesAnotaciones)
                .HasForeignKey(ra => ra.AnotacionId).IsRequired();
        }
    }
}
