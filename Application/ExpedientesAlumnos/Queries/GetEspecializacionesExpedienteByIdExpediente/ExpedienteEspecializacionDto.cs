using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetEspecializacionesExpedienteByIdExpediente
{
    public class ExpedienteEspecializacionDto : IMapFrom<ExpedienteEspecializacion>
    {
        public int Id { get; set; }
        public string IdRefEspecializacion { get; set; }
    }
}
