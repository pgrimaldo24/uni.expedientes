using MediatR;
using System;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon
{
    public class CreateMatriculaReiniciadaCommonCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public int TipoSituacionId { get; set; }
        public string MensajeSeguimiento { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
