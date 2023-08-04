using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponsePlanSuperadoErpAcademico
    {
        public ResponsePlanSuperadoErpAcademico()
        {
            CausasInsuperacion = new List<string>();
        }

        public bool EsSuperado { get; set; }
        public List<string> CausasInsuperacion { get; set; }
        public ResponseElementoSuperadoErpAcademico ElementosSuperados { get; set; }
    }
}
