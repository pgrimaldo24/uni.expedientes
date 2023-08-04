using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados
{
    public class GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery : QueryParameters, IRequest<AlumnoPuedeTitularseDto>
    {
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string IdRefUniversidad { get; set; }
        public bool? ConAsignaturas { get; set; }
        public bool? TodasAsignaturas { get; set; }
        public bool? AsignaturasSuperadas { get; set; }
        public bool? AsignaturasSuspensas { get; set; }
        public bool? AsignaturasMatriculadas { get; set; }
        public bool? AsignaturasNoPresentadas { get; set; }
    }
}
