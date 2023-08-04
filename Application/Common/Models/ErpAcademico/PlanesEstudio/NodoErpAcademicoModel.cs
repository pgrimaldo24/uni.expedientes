using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class NodoErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public virtual List<HitoErpAcademicoModel> Hitos { get; set; }
        public TipoNodoAcademicoModel Tipo { get; set; }
        public List<VersionPlanAcademicoModel> VersionesPlanes { get; set; }
    }
}
