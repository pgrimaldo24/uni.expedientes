using System.Collections.Generic;
using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasExpedientesAlumnos
{
    public class HasExpedientesAlumnosCommand : QueryParameters, IRequest<bool>
    {
        public int? FilterIdPlan { get; set; }
        public string FilterIdRefIntegracionAlumno { get; set; }
        public int? FilterIdSeguimientos { get; set; }
        public string FilterIdRefNodo { get; set; }
        public List<string> FiltersIdsRefVersionPlan { get; set; }
        public List<string> FiltersIdsRefPlan { get; set; }
    }
}
