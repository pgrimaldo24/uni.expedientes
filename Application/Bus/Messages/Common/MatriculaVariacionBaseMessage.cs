using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.Application.Bus.Messages.Common
{
    public class MatriculaVariacionBaseMessage : IMessage
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public string UniversidadIdIntegracion { get; set; }
        public int IdVariacion { get; set; }
        public string VariacionIdIntegracion { get; set; }
    }
}
