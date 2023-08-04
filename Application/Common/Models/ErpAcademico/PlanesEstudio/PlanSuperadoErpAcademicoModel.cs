using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class PlanSuperadoErpAcademicoModel
    {
        public PlanSuperadoErpAcademicoModel()
        {
            CausasInsuperacion = new List<string>();
        }

        public bool EsSuperado { get; set; }
        public List<string> CausasInsuperacion { get; set; }
        public ElementoSuperadoErpAcademicoModel ElementosSuperados { get; set; }
    }
}
