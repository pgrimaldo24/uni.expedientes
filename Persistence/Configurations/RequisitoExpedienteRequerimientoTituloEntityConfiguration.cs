using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Persistence.Configurations
{
    public class RequisitoExpedienteRequerimientoTituloEntityConfiguration : IEntityTypeConfiguration<RequisitoExpedienteRequerimientoTitulo>
    {
        public void Configure(EntityTypeBuilder<RequisitoExpedienteRequerimientoTitulo> builder)
        {
            //Relaciones a Uno
            builder
                .HasOne(e => e.TipoRelacionExpediente)
                .WithMany(e => e.RequisitosExpedientesRequerimientosTitulos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(e => e.RequisitoExpediente)
                .WithMany(e => e.RequisitosExpedientesRequerimientosTitulos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Llave primaria y Nombre de Tabla
            builder.HasKey(ll => ll.Id);
            builder.ToTable("RequisitosExpedientesRequerimientosTitulos");

            //Configuración de Llave Primaria Autonumérica.
            builder.Property(ll => ll.Id).ValueGeneratedOnAdd();
        }
    }
}
