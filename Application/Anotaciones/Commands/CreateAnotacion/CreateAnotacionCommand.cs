using MediatR;

namespace Unir.Expedientes.Application.Anotaciones.Commands.CreateAnotacion
{
    public class CreateAnotacionCommand : IRequest
    {
        public int IdExpedienteAlumno { get; set; }
        public bool EsPublica { get; set; }
        public bool EsRestringida { get; set; }
        public string[] RolesAnotaciones { get; set; }
        public string Resumen { get; set; }
        public string Mensaje { get; set; }
    }
}
