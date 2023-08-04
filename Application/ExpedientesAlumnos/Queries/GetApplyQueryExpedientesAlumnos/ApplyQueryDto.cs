using System.Collections.Generic;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos
{
    public class ApplyQueryDto : QueryParameters
    {
        public int? FilterIdExpedienteAlumno { get; set; }
        public int? FilterIdRefUniversidad { get; set; }
        public List<string> FiltersIdsRefPlan { get; set; }
        public int? FilterIdRefPlan { get; set; }
        public string FilterNombreAlumno { get; set; }
        public string FilterPrimerApellido { get; set; }
        public string FilterSegundoApellido { get; set; }
        public string FilterNroDocIdentificacion { get; set; }
        public string FilterIdRefIntegracionAlumno { get; set; }
    }
}
