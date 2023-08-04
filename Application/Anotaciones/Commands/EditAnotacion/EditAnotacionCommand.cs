using MediatR;

namespace Unir.Expedientes.Application.Anotaciones.Commands.EditAnotacion
{
    public class EditAnotacionCommand : IRequest
    {
        public int Id { get; set; }
        public int IdExpedienteAlumno { get; set; }
        public bool EsPublica { get; set; }
        public bool EsRestringida { get; set; }
        public string[] RolesAnotaciones { get; set; }
        public string Resumen { get; set; }
        public string Mensaje { get; set; }
    }
}
