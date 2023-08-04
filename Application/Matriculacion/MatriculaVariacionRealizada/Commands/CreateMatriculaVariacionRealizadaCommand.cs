using MediatR;
using System;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRealizada.Commands
{
    public class CreateMatriculaVariacionRealizadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public int[] IdsAsignaturasOfertadasExcluidas { get; set; }
        public int[] IdsAsignaturasOfertadasAdicionadas { get; set; }
        public string CausaEnumDominio { get; set; }
        public DateTime FechaHoraAlta { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
