using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio
{
    public class CanQualifyAlumnoInPlanEstudioQuery : QueryParameters, IRequest<ExpedienteAlumnoTitulacionPlanDto>
    {
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
    }
}
