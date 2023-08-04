using System.Collections.Generic;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados
{
    public class BloqueoExpedienteAlumnoDto
    {
        public BloqueoExpedienteAlumnoDto()
        {
            AccionesBloquedas = new List<AccionBloqueoDto>();
        }
        public bool Bloqueado { get; set; }
        public string Nombre { get; set; }
        public string Observacion { get; set; }
        public List<AccionBloqueoDto> AccionesBloquedas { get; set; }
    }

    public class AccionBloqueoDto
    {
        public string CodigoAccion { get; set; }
    }
}
