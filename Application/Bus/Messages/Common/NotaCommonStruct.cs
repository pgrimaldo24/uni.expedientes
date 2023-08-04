using System;

namespace Unir.Expedientes.Application.Bus.Messages.Common
{
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