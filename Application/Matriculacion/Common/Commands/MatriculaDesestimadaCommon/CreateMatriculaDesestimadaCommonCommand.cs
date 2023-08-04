using MediatR;
using System;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaDesestimadaCommon
{
    public class CreateMatriculaDesestimadaCommonCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public string UniversidadIdIntegracion { get; set; }
        public int IdVariacion { get; set; }
        public string VariacionIdIntegracion { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
        public bool EsAmpliacion { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
