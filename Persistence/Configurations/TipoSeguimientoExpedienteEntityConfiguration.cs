
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class TipoSeguimientoExpedienteEntityConfiguration : IEntityTypeConfiguration<TipoSeguimientoExpediente>
    {
        public void Configure(EntityTypeBuilder<TipoSeguimientoExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();

			//Relaciones Uno a Muchos
            builder.HasMany(r => r.Seguimientos)
                .WithOne(t => t.TipoSeguimiento)
                .HasForeignKey(i => i.TipoSeguimientoId)
                .IsRequired();

			//Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.Property(r => r.Id).HasColumnName("IdTipoSeguimientoExpediente");
            builder.ToTable("TipoSeguimientoExpediente");
            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedNever();
        }
	}
}
