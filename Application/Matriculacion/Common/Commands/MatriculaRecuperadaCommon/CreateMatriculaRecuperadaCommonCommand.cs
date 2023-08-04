using System;
using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon
{
    public class CreateMatriculaRecuperadaCommonCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public List<int> IdsAsignaturasOfertadas { get; set; }
        public List<int> IdsAsignaturasExcluidas { get; set; }
        public DateTime FechaHora { get; set; }
        public bool IsAmpliacion { get; set; }
        public bool EsVariacion { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
