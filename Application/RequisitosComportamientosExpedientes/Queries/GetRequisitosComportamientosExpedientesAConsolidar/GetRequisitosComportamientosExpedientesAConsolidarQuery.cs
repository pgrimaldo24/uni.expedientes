using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar
{
    public class GetRequisitosComportamientosExpedientesAConsolidarQuery : IRequest<List<RequisitoComportamientoExpediente>>
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public GetRequisitosComportamientosExpedientesAConsolidarQuery(
            ExpedienteAlumno expedienteAlumno)
        {
            ExpedienteAlumno = expedienteAlumno;
        }
    }
}
