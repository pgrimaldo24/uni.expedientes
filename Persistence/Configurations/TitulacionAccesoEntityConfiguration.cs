using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TitulacionAccesoEntityConfiguration : IEntityTypeConfiguration<TitulacionAcceso>
    {
        public void Configure(EntityTypeBuilder<TitulacionAcceso> builder)
        {
            builder.ToTable("TitulacionesAccesos");
            builder.Property(r => r.Titulo).IsRequired();
            builder.Property(r => r.InstitucionDocente).IsRequired();
            builder.Property(r => r.NroSemestreRealizados).IsRequired(false);
            builder.Property(r => r.TipoEstudio).IsRequired(false);
            builder.Property(r => r.IdRefTerritorioInstitucionDocente).IsRequired(false);
            builder.Property(r => r.FechaInicioTitulo).IsRequired(false);
            builder.Property(r => r.FechafinTitulo).IsRequired(false);
            builder.Property(r => r.CodigoColegiadoProfesional).IsRequired(false);
            builder.Property(r => r.IdRefInstitucionDocente).IsRequired(false);
            builder
                .HasMany(e => e.ExpedientesAlumnos)
                .WithOne(s => s.TitulacionAcceso)
                .HasForeignKey(i => i.IdTitulacionAcceso);
        }
    }
}
