using System;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ConsolidacionRequisitoExpedienteDocumento : Entity<int>
    {
        public static string[] Extensiones = new string[] { "doc", "docx", "pdf", "jpg", "jpeg", "png", "gif" };
        public const int Capacidad = 1024;
        /// <summary>
        /// Nombre del fichero
        /// </summary>
        [StringLength(500)]
        public string Fichero { get; set; }

        /// <summary>
        /// Fecha de recepción del fichero
        /// </summary>
        public DateTime FechaFichero { get; set; }

        /// <summary>
        /// Fichero en el gestor documental
        /// </summary>
        public string IdRefDocumento { get; set; }

        /// <summary>
        /// Indica si el fichero ha sido o no validado; valor false, por defecto
        /// </summary>
        public bool FicheroValidado { get; set; }

        // Propiedades de navegación
        public int? RequisitoExpedienteDocumentoId { get; set; }
        public virtual RequisitoExpedienteDocumento RequisitoExpedienteDocumento { get; set; }
        public int ConsolidacionRequisitoExpedienteId { get; set; }
        public virtual ConsolidacionRequisitoExpediente ConsolidacionRequisitoExpediente { get; set; }
    }
}
