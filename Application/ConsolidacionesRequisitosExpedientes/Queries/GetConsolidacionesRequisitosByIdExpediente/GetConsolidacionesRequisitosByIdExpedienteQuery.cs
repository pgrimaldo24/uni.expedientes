using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionesRequisitosByIdExpediente
{
    public class GetConsolidacionesRequisitosByIdExpedienteQuery : IRequest<List<ConsolidacionRequisitoExpedienteDto>>
    {
        public int IdExpedienteAlumno { get; set; }
        public GetConsolidacionesRequisitosByIdExpedienteQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
