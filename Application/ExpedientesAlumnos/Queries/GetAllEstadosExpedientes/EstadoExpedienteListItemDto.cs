using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllEstadosExpedientes
{
    public class EstadoExpedienteListItemDto : IMapFrom<EstadoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
