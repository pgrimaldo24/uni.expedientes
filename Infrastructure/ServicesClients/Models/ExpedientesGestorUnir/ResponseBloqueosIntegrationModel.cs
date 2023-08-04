using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseBloqueosIntegrationModel
    {
        public string Nombre { get; set; }
        public string Observacion { get; set; }
        public List<ResponseAccionesBloqueadasIntegrationModel> AccionesBloquedas { get; set; }
    }

    public class ResponseAccionesBloqueadasIntegrationModel
    {
        public string CodigoAccion { get; set; }
    }
}
