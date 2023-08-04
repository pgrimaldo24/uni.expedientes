using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RequisitoExpedienteRequerimientoTitulo : Entity<int>
    {
        public bool RequiereMatricularse { get; set; }
        public virtual TipoRelacionExpediente TipoRelacionExpediente { get; set; }
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
    }
}
