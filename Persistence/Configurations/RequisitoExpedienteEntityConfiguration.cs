using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RequisitoExpedienteEntityConfiguration : IEntityTypeConfiguration<RequisitoExpediente>
    {
        public void Configure(EntityTypeBuilder<RequisitoExpediente> builder)
        {
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.Nombre).IsRequired();
            builder.Property(r => r.Orden).IsRequired();
            builder.Property(r => r.Descripcion).IsRequired(false);
            builder.Property(r => r.EstaVigente).HasDefaultValue(true).IsRequired();
            builder.Property(r => r.RequeridaParaTitulo).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.RequiereDocumentacion).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.EnviarEmailAlumno).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.RequeridaParaPago).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.EstaRestringida).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.EsSancion).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.EsLogro).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.EsCertificado).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.CertificadoIdioma).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.RequiereTextoAdicional).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.Bloqueado).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.IdRefModoRequerimientoDocumentacion).IsRequired(false);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("RequisitosExpedientes");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.EstadoExpediente).WithMany(t => t.RequisitosExpedientes);
        }
    }
}
