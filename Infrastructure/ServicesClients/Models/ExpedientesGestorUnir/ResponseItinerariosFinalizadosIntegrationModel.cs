using System;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseItinerariosFinalizadosIntegrationModel
    {
        public ResponseTipoItinerarioIntegrationModel TipoItinerario { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaFin { get; set; }
        public string IdEspecializacionErp { get; set; }
    }
}
