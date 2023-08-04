using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class ConsolidacionRequisitoExpedienteDocumentoEntityConfiguration : IEntityTypeConfiguration<ConsolidacionRequisitoExpedienteDocumento>
    {
        public void Configure(EntityTypeBuilder<ConsolidacionRequisitoExpedienteDocumento> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Fichero).IsRequired();
            builder.Property(r => r.FechaFichero).IsRequired();
            builder.Property(r => r.FicheroValidado).HasDefaultValue(false).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("ConsolidacionesRequisitosExpedientesDocumentos");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.RequisitoExpedienteDocumento).WithMany(t => t.ConsolidacionesRequisitosExpedientesDocumentos)
                .HasForeignKey(r => r.RequisitoExpedienteDocumentoId);
            builder.HasOne(s => s.ConsolidacionRequisitoExpediente).WithMany(t => t.ConsolidacionesRequisitosExpedientesDocumentos)
                .HasForeignKey(r => r.ConsolidacionRequisitoExpedienteId).IsRequired();
        }
	}
}
