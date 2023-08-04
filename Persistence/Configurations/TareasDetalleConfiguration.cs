using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public partial class TareasDetalleConfiguration : IEntityTypeConfiguration<TareaDetalle>
    {
        public void Configure(EntityTypeBuilder<TareaDetalle> builder)
        {
            //Llave primaria y Nombre de Tabla
            builder.HasKey(td => td.Id);
            builder.ToTable("TareasDetalle");

            //Mapeo de Propiedades y Columnas
            builder.Property(r => r.Id).IsRequired();
            builder.Property(r => r.TareaId).IsRequired();
            builder.Property(r => r.ExpedienteId).IsRequired();
            builder.Property(r => r.FechaInicio).IsRequired();
            builder.Property(r => r.FechaFin).IsRequired(false);
            builder.Property(r => r.CompletadaOk).IsRequired();
            builder.Property(r => r.Mensaje).IsRequired(false);

            //Relaciones a uno
            builder.HasOne(d => d.Tarea)
                .WithMany(p => p.TareasDetalle)
                .HasForeignKey(d => d.TareaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TareasDetalle_Tareas");

            builder.HasOne(s => s.Expediente)
                .WithMany(t => t.TareasDetalle)
                .HasForeignKey(t => t.ExpedienteId).IsRequired();

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(td => td.Id).ValueGeneratedOnAdd();
        }
    }
}
