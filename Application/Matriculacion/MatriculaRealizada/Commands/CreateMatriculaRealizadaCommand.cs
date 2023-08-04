using MediatR;
using System;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaRealizada.Commands
{
    public class CreateMatriculaRealizadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
