    using System.ComponentModel.DataAnnotations;
    using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TransicionEstadoExpediente : Entity<int>
    {
        // Propiedades Primitivas
        /// <summary>
        /// Indica si la transición puede o no realizarse a la inversa; por defecto es False
        /// </summary>
        public bool EsInversa { get; set; }

        /// <summary>
        /// Indica si la transición es manual (la realiza el usuario) o automática (la realiza el sistema); por defecto es False
        /// </summary>
        public bool EsManual { get; set; }

        /// <summary>
        /// Nombre de la transición usada luego en el desarrollo
        /// </summary>
        [StringLength(100)]
        public string Nombre { get; set; }

        /// <summary>
        /// Orden de la transición en el proceso
        /// </summary>
        public int? Orden { get; set; }

        /// <summary>
        /// Por defecto false, indica que sólo un administrador lleva a cabo la transición
        /// </summary>
        public bool SoloAdmin { get; set; }

        // Propiedades de Navegación
        public virtual EstadoExpediente EstadoOrigen { get; set; }
        public virtual EstadoExpediente EstadoDestino { get; set; }

    }
}
