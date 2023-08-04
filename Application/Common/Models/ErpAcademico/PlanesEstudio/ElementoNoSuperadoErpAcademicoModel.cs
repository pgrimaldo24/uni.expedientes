using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class ElementoNoSuperadoErpAcademicoModel
    {
        public ElementoNoSuperadoErpAcademicoModel()
        {
            BloquesNoSuperados = new List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>();
            AsignaturasPlanNoSuperadas = new List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel>();
        }

        public List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> BloquesNoSuperados { get; set; }
        public List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel> AsignaturasPlanNoSuperadas { get; set; }
    }
}
