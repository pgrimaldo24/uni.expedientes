using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.Application.Bus.Messages.Common
{
    public class MatriculaBaseMessage : IMessage
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public string UniversidadIdIntegracion { get; set; }
    }
}
