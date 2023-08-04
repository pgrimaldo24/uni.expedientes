using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RequisitoExpedienteFileTypeEntityConfiguration : IEntityTypeConfiguration<RequisitoExpedienteFileType>
    {
        public void Configure(EntityTypeBuilder<RequisitoExpedienteFileType> builder)
		{
            //Propiedades Escalares Opcionales y Requeridas
            builder.Property(r => r.IdRefFileType).IsRequired();

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("RequisitosExpedientesFileType");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();

            //Relaciones a Uno
            builder.HasOne(s => s.RequisitoExpediente).WithMany(t => t.RequisitosExpedientesFilesType).IsRequired();
        }
	}
}
