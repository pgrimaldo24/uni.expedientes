using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class SeguimientoExpedienteEntityConfiguration : IEntityTypeConfiguration<SeguimientoExpediente>
    {
        public void Configure(EntityTypeBuilder<SeguimientoExpediente> builder)
		{
			//Mapeo de Propiedades y Columnas
            builder.Property(r => r.Id).HasColumnName("IdSeguimientoExpediente");
            builder.Property(r => r.TipoSeguimientoId).HasColumnName("IdTipoSeguimientoExpediente");
            builder.Property(r => r.ExpedienteAlumnoId).HasColumnName("IdExpedienteAlumno");

			//Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Fecha).IsRequired();
            builder.Property(r => r.IdRefTrabajador).HasMaxLength(100);
            builder.Property(r => r.Descripcion);
            builder.Property(r => r.IdCuentaSeguridad).HasMaxLength(250);
            builder.Property(r => r.OrigenExterno).HasMaxLength(250);
            builder.Property(r => r.Mensaje).HasMaxLength(250);

            //Relaciones a Uno
            builder.HasOne(s => s.TipoSeguimiento).WithMany(t => t.Seguimientos).HasForeignKey(t => t.TipoSeguimientoId).IsRequired();
            builder.HasOne(s => s.ExpedienteAlumno).WithMany(t => t.Seguimientos).HasForeignKey(t => t.ExpedienteAlumnoId).IsRequired();
            builder.HasOne(s => s.Estado).WithMany(t => t.Seguimientos).HasForeignKey(t => t.EstadoId);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("SeguimientoExpediente");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();
        }
	}
}
