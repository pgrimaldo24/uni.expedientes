using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public partial class TareasConfiguration : IEntityTypeConfiguration<Tarea>
    {
        public void Configure(EntityTypeBuilder<Tarea> builder)
        {
            builder.ToTable("Tareas");
            builder.HasKey(e => e.Id);
            builder.Property(r => r.JobId).IsRequired();
            builder.Property(r => r.FechaInicio).IsRequired();
            builder.Property(r => r.FechaFin).IsRequired(false);
            builder.Property(r => r.Total).IsRequired();
            builder.Property(r => r.Completadas).IsRequired();
            builder.Property(r => r.Fallidas).IsRequired();
            builder.Property(r => r.IdRefEstudio).IsRequired(false);
            builder.Property(r => r.IdRefUniversidad).IsRequired(false);
            
            //Relaciones uno a muchos
            builder
                .HasMany(e => e.TareasDetalle)
                .WithOne(s => s.Tarea)
                .HasForeignKey(i => i.TareaId);
        }   
    }
}
