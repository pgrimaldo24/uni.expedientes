using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RequisitoExpedienteDocumentoEntityConfiguration : IEntityTypeConfiguration<RequisitoExpedienteDocumento>
    {
        public void Configure(EntityTypeBuilder<RequisitoExpedienteDocumento> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.NombreDocumento).IsRequired();
            builder.Property(r => r.DocumentoObligatorio).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.DocumentoEditable).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.DocumentoSecurizado).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.RequiereAceptacionAlumno).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.IdRefPlantilla).IsRequired(false);
            builder.Property(r => r.DocumentoClasificacion).IsRequired().HasMaxLength(100);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("RequisitosExpedientesDocumentos");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.RequisitoExpediente).WithMany(t => t.RequisitosExpedientesDocumentos).IsRequired(false);
        }
    }
}
