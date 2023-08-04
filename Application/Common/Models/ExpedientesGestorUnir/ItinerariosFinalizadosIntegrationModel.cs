using System;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class ItinerariosFinalizadosIntegrationModel
    {
        public TipoItinerarioIntegrationModel TipoItinerario { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaFin { get; set; }
        public string IdEspecializacionErp { get; set; }
    }
}
