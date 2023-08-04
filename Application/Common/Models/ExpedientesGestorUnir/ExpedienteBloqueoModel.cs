using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class ExpedienteBloqueoModel
    {
        public int CodigoResultado { get; set; }
        public int IdAlumno { get; set; }
        public int? IdEstudioGestor { get; set; }
        public int? IdPlanErp { get; set; }
        public bool Bloqueado { get; set; }
        public List<MotivoBloqueado> Motivos { get; set; }
    }

    public class MotivoBloqueado
    {
        public string Nombre { get; set; }
        public string Observacion { get; set; }
        public List<AccionBloqueada> AccionesBloqueadas { get; set; }
    }

    public class AccionBloqueada
    {
        public string Codigo { get; set; }
    }
}
