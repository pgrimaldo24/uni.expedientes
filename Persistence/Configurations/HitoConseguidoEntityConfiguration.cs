using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class HitoConseguidoEntityConfiguration : IEntityTypeConfiguration<HitoConseguido>
    {
        public void Configure(EntityTypeBuilder<HitoConseguido> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).HasMaxLength(250).IsRequired();
            builder.Property(r => r.FechaInicio).IsRequired();
            builder.Property(r => r.FechaFin).IsRequired(false);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("HitosConseguidos");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.ExpedienteAlumno).WithMany(t => t.HitosConseguidos)
                .HasForeignKey(r => r.ExpedienteAlumnoId).IsRequired();
            builder.HasOne(s => s.TipoConseguido).WithMany(t => t.HitosConseguidos)
                .HasForeignKey(r => r.TipoConseguidoId).IsRequired();
            builder.HasOne(s => s.ExpedienteEspecializacion).WithMany(t => t.HitosConseguidos)
                .HasForeignKey(r => r.ExpedienteEspecializacionId);
        }
	}
}
