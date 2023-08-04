using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ExpedienteEspecializacionEntityConfiguration : IEntityTypeConfiguration<ExpedienteEspecializacion>
    {
        public void Configure(EntityTypeBuilder<ExpedienteEspecializacion> builder)
        {
            builder.ToTable("ExpedientesEspecializaciones");
            builder.HasKey(e => e.Id);
            builder.HasAlternateKey(e => new {e.ExpedienteAlumnoId, e.IdRefEspecializacion});
            builder.Property(r => r.IdRefEspecializacion).IsRequired();

            builder.HasOne(s => s.ExpedienteAlumno).WithMany(t => t.ExpedientesEspecializaciones)
                .HasForeignKey(t => t.ExpedienteAlumnoId).IsRequired();
        }
    }
}
