using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class SituacionAsignatura : Entity<int>
    {
        public const int Matriculada = 1;
        public const int NoPresentada = 2;
        public const int NoSuperada = 3;
        public const int Superada = 4;
        public const int MatriculaHonor = 5;
        public const int Anulada = 6;
        public const int Reconocida = 7;
        public const int Adaptada = 8;
        /// <summary>
        /// Nombre de la situación
        /// </summary>
        public string Nombre { get; set; }
        public virtual ICollection<AsignaturaExpediente> AsignaturasExpedientes { get; set; }
    }
}
