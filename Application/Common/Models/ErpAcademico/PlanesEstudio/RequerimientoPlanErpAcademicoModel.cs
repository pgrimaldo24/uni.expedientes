using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class RequerimientoPlanErpAcademicoModel
    {
        public RequerimientoPlanErpAcademicoModel()
        {
            RequerimientosTiposAsignaturas = new List<RequerimientoTipoAsignaturaErpAcademicoModel>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CreditosRequerido { get; set; }
        public double? CreditosObtenidos { get; set; }
        public List<RequerimientoTipoAsignaturaErpAcademicoModel> RequerimientosTiposAsignaturas { get; set; }
    }
}
