using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoHitoConseguido : Entity<int>
    {
        public const int PreMatriculado = 1;
        public const int PrimeraMatricula = 2;
        public const int Matriculado = 3;
        public const int Cerrado = 6;
        public const int TrabajoFinEstudio = 8;
        public const int Especializacion = 9;
        public const int TasasAbonadas = 10;
        public const int Desestimado = 13;
        public const int Titulacion = 14;
        public const int Anulado = 15;
        // Propiedades Primitivas
        /// <summary>
        /// Nombre del tipo de hito conseguido
        /// </summary>
        public string Nombre { get; set; }
        public string Icono { get; set; }

        // Propiedades de Navegación
        public virtual ICollection<HitoConseguido> HitosConseguidos { get; set; }
    }
}
