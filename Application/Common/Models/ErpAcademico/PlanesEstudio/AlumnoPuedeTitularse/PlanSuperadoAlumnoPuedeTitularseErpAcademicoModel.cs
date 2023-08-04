using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse
{
    public class PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
    {
        public PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel()
        {
            CausasInsuperacion = new List<string>();
        }

        public bool EsSuperado { get; set; }
        public List<string> CausasInsuperacion { get; set; }
        public ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel ElementosSuperados { get; set; }
    }
}
