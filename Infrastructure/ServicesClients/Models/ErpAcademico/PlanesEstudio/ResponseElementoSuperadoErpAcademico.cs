using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseElementoSuperadoErpAcademico
    {
        public ResponseElementoSuperadoErpAcademico()
        {
            NodosAlcanzados = new List<ResponseNodoErpAcademico>();
            NodosActuales = new List<int>();
            HitosObtenidos = new List<ResponseHitoErpAcademico>();
            ArcosSuperados = new List<ResponseArcoErpAcademico>();
            BloquesSuperados = new List<ResponseBloqueSuperadoErpAcademico>();
            AsignaturasPlanSuperadas = new List<ResponseAsignaturaPlanErpAcademico>();
            RequerimientosSuperados = new List<ResponseRequerimientoPlanErpAcademico>();
            TrayectosPlanSuperados = new List<ResponseTrayectoPlanErpAcademico>();
        }

        public List<ResponseNodoErpAcademico> NodosAlcanzados { get; set; }
        public List<int> NodosActuales { get; set; }
        public List<ResponseHitoErpAcademico> HitosObtenidos { get; set; }
        public List<ResponseArcoErpAcademico> ArcosSuperados { get; set; }
        public List<ResponseBloqueSuperadoErpAcademico> BloquesSuperados { get; set; }
        public List<ResponseAsignaturaPlanErpAcademico> AsignaturasPlanSuperadas { get; set; }
        public List<ResponseRequerimientoPlanErpAcademico> RequerimientosSuperados { get; set; }
        public List<ResponseTrayectoPlanErpAcademico> TrayectosPlanSuperados { get; set; }
    }
}
