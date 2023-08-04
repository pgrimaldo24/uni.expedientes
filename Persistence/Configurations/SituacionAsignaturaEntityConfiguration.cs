using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    internal class SituacionAsignaturaEntityConfiguration : IEntityTypeConfiguration<SituacionAsignatura>
    {
        public void Configure(EntityTypeBuilder<SituacionAsignatura> builder)
        {
            builder.ToTable("SituacionesAsignaturas");
            builder.HasKey(sa => sa.Id);
            builder.Property(sa => sa.Nombre).IsRequired().HasMaxLength(50);
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
    }
}
