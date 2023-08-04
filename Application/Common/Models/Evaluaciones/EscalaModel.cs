
namespace Unir.Expedientes.Application.Common.Models.Evaluaciones
{
    public class EscalaModel
    {
        public int IdEscala { get; set; }
        public int IdVersion { get; set; }
        public string NombreEscala { get; set; }
        public string NombreVersion { get; set; }
        public int Version { get; set; }
        public bool Vigente { get; set; }
        public string Estado { get; set; }
    }
}
