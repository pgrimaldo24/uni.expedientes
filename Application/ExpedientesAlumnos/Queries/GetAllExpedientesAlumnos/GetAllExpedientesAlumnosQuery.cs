using MediatR;
using System.Collections.Generic;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos
{
    public class GetAllExpedientesAlumnosQuery : QueryParameters, IRequest<ExpedienteAlumnoListItemDto[]>
    {
        public int? FilterIdPlan { get; set; }
        public string FilterIdRefIntegracionAlumno { get; set; }
        public List<string> FilterIdsRefIntegracionAlumnos { get; set; }
        public int? FilterIdSeguimientos { get; set; }
        public string FilterIdRefNodo { get; set; }
        public List<string> FiltersIdsRefVersionPlan { get; set; }
        public List<string> FiltersIdsRefPlan { get; set; }
        public int? FilterIdExpedienteAlumno { get; set; }
        public List<int> FiltersIdsExpedientesAlumnos { get; set; }
    }
}
