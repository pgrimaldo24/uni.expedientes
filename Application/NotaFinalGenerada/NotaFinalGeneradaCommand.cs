using System;
using System.Collections.Generic;
using MediatR;

namespace Unir.Expedientes.Application.NotaFinalGenerada
{
    public class NotaFinalGeneradaCommand : IRequest
    {
        public string Plataforma { get; set; }
        public bool Provisional { get; set; }
        public int IdCurso { get; set; }
        public int IdUsuarioPublicadorConfirmador { get; set; }
        public List<NotaCommonStruct> Notas { get; set; }
    }

    public class NotaCommonStruct
    {
        public int IdAlumno { get; set; }
        public string Convocatoria { get; set; }
        public double Calificacion { get; set; }
        public bool EsMatriculaHonor { get; set; }
        public bool NoPresentado { get; set; }
        public DateTime? FechaPublicado { get; set; }
        public DateTime? FechaConfirmado { get; set; }
    }
}