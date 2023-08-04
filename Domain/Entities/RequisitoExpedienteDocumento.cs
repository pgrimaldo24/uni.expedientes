using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RequisitoExpedienteDocumento : Entity<int>
    {
        public const string CodigoClasificacion = "J.03.01.01";
        /// <summary>
        /// Nombre del documento
        /// </summary>
        public string NombreDocumento { get; set; }

        /// <summary>
        /// Indica si el documento es o no obligatorio; valor false, por defecto
        /// </summary>
        public bool DocumentoObligatorio { get; set; }

        /// <summary>
        /// Indica si el documento se puede o no editar; valor false, por defecto
        /// </summary>
        public bool DocumentoEditable { get; set; }

        /// <summary>
        /// Indica si el documento tiene que ser securizado o no al guardarlo en el sistema; valor false, por defecto
        /// </summary>
        public bool DocumentoSecurizado { get; set; }

        /// <summary>
        /// Indica si el documento tiene que ser aceptado o no por el alumno; valor false, por defecto
        /// </summary>
        public bool RequiereAceptacionAlumno { get; set; }

        /// <summary>
        /// Fichero en el gestor documental
        /// </summary>
        public string IdRefPlantilla { get; set; }

        /// <summary>
        /// Código de la clasificación documental
        /// </summary>
        public string DocumentoClasificacion { get; set; }

        // Propiedades de Navegación
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpedienteDocumento> ConsolidacionesRequisitosExpedientesDocumentos { get; set; }
    }
}
