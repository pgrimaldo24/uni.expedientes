using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse
{
    public class ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
    {
        public ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel()
        {
            NodosAlcanzados = new List<NodoErpAcademicoModel>();
            NodosActuales = new List<int>();
            HitosObtenidos = new List<HitoErpAcademicoModel>();
            Arcos = new List<ArcoAlumnoPuedeTitulularseErpAcademicoModel>();
            BloquesSuperados = new List<BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel>();
            AsignaturasPlanSuperadas = new List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel>();
            RequerimientosSuperados = new List<RequerimientoPlanErpAcademicoModel>();
            TrayectosPlanSuperados = new List<TrayectoPlanErpAcademicoModel>();
        }

        public List<NodoErpAcademicoModel> NodosAlcanzados { get; set; }
        public List<int> NodosActuales { get; set; }
        public List<HitoErpAcademicoModel> HitosObtenidos { get; set; }
        public List<ArcoAlumnoPuedeTitulularseErpAcademicoModel> Arcos { get; set; }
        public List<BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel> BloquesSuperados { get; set; }
        public List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel> AsignaturasPlanSuperadas { get; set; }
        public List<RequerimientoPlanErpAcademicoModel> RequerimientosSuperados { get; set; }
        public List<TrayectoPlanErpAcademicoModel> TrayectosPlanSuperados { get; set; }
    }
}
