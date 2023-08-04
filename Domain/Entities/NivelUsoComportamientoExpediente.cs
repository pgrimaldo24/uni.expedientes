using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class NivelUsoComportamientoExpediente : Entity<int>
    {
        /// <summary>
        /// Identificador de la universidad
        /// </summary>
        [StringLength(50)]
        public string IdRefUniversidad { get; set; }

        /// <summary>
        /// Acrónimo de la universidad
        /// </summary>
        [StringLength(15)]
        public string AcronimoUniversidad { get; set; }

        /// <summary>
        /// Identificador del tipo de estudio
        /// </summary>
        [StringLength(50)]
        public string IdRefTipoEstudio { get; set; }

        /// <summary>
        /// Nombre del tipo de estudio
        /// </summary>
        [StringLength(100)]
        public string NombreTipoEstudio { get; set; }

        /// <summary>
        /// Identificador del estudio
        /// </summary>
        [StringLength(50)]
        public string IdRefEstudio { get; set; }

        /// <summary>
        /// Nombre del estudio
        /// </summary>
        [StringLength(200)]
        public string NombreEstudio { get; set; }

        /// <summary>
        /// Identificador del plan de estudios
        /// </summary>
        [StringLength(50)]
        public string IdRefPlan { get; set; }

        /// <summary>
        /// Nombre del plan de estudios
        /// </summary>
        [StringLength(250)]
        public string NombrePlan { get; set; }

        /// <summary>
        /// Identificador del tipo de asignatura
        /// </summary>
        [StringLength(50)]
        public string IdRefTipoAsignatura { get; set; }

        /// <summary>
        /// Nombre del tipo de asignatura
        /// </summary>
        [StringLength(200)]
        public string NombreTipoAsignatura { get; set; }

        /// <summary>
        /// Identificador de la asignatura perteneciente al plan de estudios
        /// </summary>
        [StringLength(50)]
        public string IdRefAsignaturaPlan { get; set; }

        /// <summary>
        /// Identificador de la asignatura
        /// </summary>
        [StringLength(50)]
        public string IdRefAsignatura { get; set; }

        /// <summary>
        /// Nombre de la asignatura ofertada
        /// </summary>
        [StringLength(200)]
        public string NombreAsignatura { get; set; }

        // Propiedades de Navegación
        public int TipoNivelUsoId { get; set; }
        public virtual TipoNivelUso TipoNivelUso { get; set; }
        public int ComportamientoExpedienteId { get; set; }
        public virtual ComportamientoExpediente ComportamientoExpediente { get; set; }
    }
}
