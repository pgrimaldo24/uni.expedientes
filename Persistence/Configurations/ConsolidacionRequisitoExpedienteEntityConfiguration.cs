using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ConsolidacionRequisitoExpedienteEntityConfiguration : IEntityTypeConfiguration<ConsolidacionRequisitoExpediente>
    {
        public void Configure(EntityTypeBuilder<ConsolidacionRequisitoExpediente> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.EsDocumentacionFisica).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.DocumentacionRecibida).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.EnviadaPorAlumno).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.FechaCambioEstado).IsRequired();
            builder.Property(r => r.Fecha).IsRequired(false);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("ConsolidacionesRequisitosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(r => r.EstadoRequisitoExpediente).WithMany(i => i.ConsolidacionesRequisitosExpedientes)
                .HasForeignKey(r => r.EstadoRequisitoExpedienteId);
            builder.HasOne(r => r.RequisitoExpediente).WithMany(i => i.ConsolidacionesRequisitosExpedientes)
                .HasForeignKey(r => r.RequisitoExpedienteId).IsRequired();
            builder.HasOne(r => r.ExpedienteAlumno).WithMany(i => i.ConsolidacionesRequisitosExpedientes)
                .HasForeignKey(r => r.ExpedienteAlumnoId).IsRequired();
            builder.HasOne(r => r.TipoRequisitoExpediente).WithMany(i => i.ConsolidacionesRequisitosExpedientes)
                .HasForeignKey(r => r.TipoRequisitoExpedienteId).IsRequired();
            builder.HasOne(r => r.CausaEstadoRequisitoConsolidadaExpediente).WithMany(i => i.ConsolidacionesRequisitosExpedientes)
                .HasForeignKey(r => r.CausaEstadoRequisitoConsolidadaExpedienteId);
        }
    }
}
