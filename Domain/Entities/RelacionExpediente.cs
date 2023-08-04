using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RelacionExpediente : Entity<int>
    {
        public int ExpedienteAlumnoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumno { get; set; }
        public int ExpedienteAlumnoRelacionadoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumnoRelacionado { get; set; }
        public int TipoRelacionId { get; set; }
        public virtual TipoRelacionExpediente TipoRelacion { get; set; }
    }
}
