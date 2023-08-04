using System;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public partial class TareaDetalle : Entity<int>
    {
        public int TareaId { get; set; }
        public int ExpedienteId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool CompletadaOk { get; set; }
        public string Mensaje { get; set; }

        public virtual ExpedienteAlumno Expediente { get; set; }
        public virtual Tarea Tarea { get; set; }
    }
}