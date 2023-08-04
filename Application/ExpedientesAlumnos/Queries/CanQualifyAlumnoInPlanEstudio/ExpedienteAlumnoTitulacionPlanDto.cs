using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio
{
    public class ExpedienteAlumnoTitulacionPlanDto
    {
        public ExpedienteAlumnoTitulacionPlanDto()
        {
            CausasFalloMatriculas = new List<string>();
        }

        public bool PuedeTitular { get; set; }
        public PlanSuperadoErpAcademicoModel EsPlanSuperado { get; set; }
        public bool MatriculasOk { get; set; }
        public List<string> CausasFalloMatriculas { get; set; }

    }
}
