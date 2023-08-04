using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados
{
    public class AlumnoPuedeTitularseDto
    {
        public AlumnoPuedeTitularseDto()
        {
            Errores = new List<string>();
            CausasFalloMatriculas = new List<string>();
            ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel();
        }

        public bool PuedeTitular { get; set; }
        public PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel EsPlanSuperado { get; set; }
        public ElementoNoSuperadoErpAcademicoModel ElementosNoSuperados { get; set; }
        public bool MatriculasOk { get; set; }
        public List<string> CausasFalloMatriculas { get; set; }
        public AsignaturaPlanErpAcademicoModel Asignatura { get; set; }
        public ExpedicionDto Expedicion { get; set; }
        public bool Bloqueado { get; set; }
        public BloqueoExpedienteAlumnoDto Bloqueos { get; set; }
        public List<string> Errores { get; set; }
    }
}
