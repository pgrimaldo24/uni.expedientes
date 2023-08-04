using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class ElementoSuperadoErpAcademicoModel
    {
        public ElementoSuperadoErpAcademicoModel()
        {
            NodosAlcanzados = new List<NodoErpAcademicoModel>();
            NodosActuales = new List<int>();
            HitosObtenidos = new List<HitoErpAcademicoModel>();
            ArcosSuperados = new List<ArcoErpAcademicoModel>();
            BloquesSuperados = new List<BloqueSuperadoErpAcademicoModel>();
            AsignaturasPlanSuperadas = new List<AsignaturaPlanErpAcademicoModel>();
            RequerimientosSuperados = new List<RequerimientoPlanErpAcademicoModel>();
            TrayectosPlanSuperados = new List<TrayectoPlanErpAcademicoModel>();
        }

        public List<NodoErpAcademicoModel> NodosAlcanzados { get; set; }
        public List<int> NodosActuales { get; set; }
        public List<HitoErpAcademicoModel> HitosObtenidos { get; set; }
        public List<ArcoErpAcademicoModel> ArcosSuperados { get; set; }
        public List<BloqueSuperadoErpAcademicoModel> BloquesSuperados { get; set; }
        public List<AsignaturaPlanErpAcademicoModel> AsignaturasPlanSuperadas { get; set; }
        public List<RequerimientoPlanErpAcademicoModel> RequerimientosSuperados { get; set; }
        public List<TrayectoPlanErpAcademicoModel> TrayectosPlanSuperados { get; set; }
    }
}
