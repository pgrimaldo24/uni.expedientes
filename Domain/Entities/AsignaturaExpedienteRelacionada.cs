using System;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class AsignaturaExpedienteRelacionada : Entity<int>
    {
        public int AsignaturaExpedienteOrigenId { get; set; }
        public virtual AsignaturaExpediente AsignaturaExpedienteOrigen { get; set; }
        public int AsignaturaExpedienteDestinoId { get; set; }
        public virtual AsignaturaExpediente AsignaturaExpedienteDestino { get; set; }
        public bool Reconocida { get; set; }
        public bool Adaptada { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
