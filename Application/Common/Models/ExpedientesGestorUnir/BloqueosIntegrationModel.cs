using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class BloqueosIntegrationModel
    {
        public string Nombre { get; set; }
        public string Observacion { get; set; }
        public List<AccionesBloqueadasIntegrationModel> AccionesBloquedas { get; set; }
    }

    public class AccionesBloqueadasIntegrationModel
    {
        public string CodigoAccion { get; set; }
    }
}
