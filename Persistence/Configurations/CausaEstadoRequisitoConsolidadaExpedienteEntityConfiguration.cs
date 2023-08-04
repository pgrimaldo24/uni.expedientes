using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class CausaEstadoRequisitoConsolidadaExpedienteEntityConfiguration : IEntityTypeConfiguration<CausaEstadoRequisitoConsolidadaExpediente>
    {
        public void Configure(EntityTypeBuilder<CausaEstadoRequisitoConsolidadaExpediente> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("CausasEstadosRequisitosConsolidadasExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.EstadoRequisitoExpediente).WithMany(t => t.CausasEstadosRequisitosConsolidadasExpediente).IsRequired();
            builder.HasOne(s => s.RequisitoExpediente).WithMany(t => t.CausasEstadosRequisitosConsolidadasExpediente).IsRequired(false);
        }
	}
}
