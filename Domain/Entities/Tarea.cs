using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class Tarea : Entity<int>
    {
        public Tarea()
        {
            TareasDetalle = new HashSet<TareaDetalle>();
        }

        public int JobId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Total { get; set; }
        public int Completadas { get; set; }
        public int Fallidas { get; set; }
        public int? IdRefUniversidad { get; set; }
        public int? IdRefEstudio { get; set; }

        public virtual ICollection<TareaDetalle> TareasDetalle { get; set; }
    }
}