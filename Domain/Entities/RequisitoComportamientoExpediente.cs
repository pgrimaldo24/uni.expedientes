using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RequisitoComportamientoExpediente : Entity<int>
    {
        // Propiedades de Navegación
        public int ComportamientoExpedienteId { get; set; }
        public virtual ComportamientoExpediente ComportamientoExpediente { get; set; }
        public int RequisitoExpedienteId { get; set; }
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
        public int TipoRequisitoExpedienteId { get; set; }
        public virtual TipoRequisitoExpediente TipoRequisitoExpediente { get; set; }
    }
}
