using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseExpedienteBloqueoGestor
    {
        [JsonPropertyName("codigoResultado")]
        public int CodigoResultado { get; set; }

        [JsonPropertyName("idAlumno")]
        public int IdAlumno { get; set; }

        [JsonPropertyName("idEstudioGestor")]
        public int? IdEstudioGestor { get; set; }

        [JsonPropertyName("idPlanErp")]
        public int? IdPlanErp { get; set; }

        [JsonPropertyName("bloqueado")]
        public bool Bloqueado { get; set; }

        [JsonPropertyName("motivos")]
        public List<ResponseMotivoBloqueado> Motivos { get; set; }
    }

    public class ResponseMotivoBloqueado
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("observacion")]
        public string Observacion { get; set; }

        [JsonPropertyName("accionesBloqueadas")]
        public List<ResponseAccionBloqueada> AccionesBloqueadas { get; set; }
    }

    public class ResponseAccionBloqueada
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }
    }
}
