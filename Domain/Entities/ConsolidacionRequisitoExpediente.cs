using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ConsolidacionRequisitoExpediente : Entity<int>
    {
        /// <summary>
        /// Indica si la documentación se ha recibido o no en formato físico; valor false, por defecto
        /// </summary>
        public bool EsDocumentacionFisica { get; set; }

        /// <summary>
        /// Indica si la documentación se ha recibido o no; valor false, por defecto
        /// </summary>
        public bool DocumentacionRecibida { get; set; }

        /// <summary>
        /// Indica si la documentación la enviado o no el alumno; valor false, por defecto
        /// </summary>
        public bool EnviadaPorAlumno { get; set; }

        /// <summary>
        /// Fecha del cambio de estado
        /// </summary>
        public DateTime FechaCambioEstado { get; set; }

        /// <summary>
        /// Identificador del idioma cuando se trata de un certificado de idioma
        /// </summary>
        [StringLength(50)]
        public string IdRefIdioma { get; set; }

        /// <summary>
        /// Siglas del idioma
        /// </summary>
        [StringLength(10)]
        public string SiglasIdioma { get; set; }

        /// <summary>
        /// Nombre del idioma
        /// </summary>
        [StringLength(50)]
        public string NombreIdioma { get; set; }

        /// <summary>
        /// Nivel de certificación del idioma: A1, A2, B1, B2, C1, C2
        /// </summary>
        [StringLength(50)]
        public string NivelIdioma { get; set; }

        /// <summary>
        /// Texto adicional que se añade al requisito
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Fecha adicional a la consolidación
        /// </summary>
        public DateTime? Fecha { get; set; }

        // Propiedades de navegación
        public int? EstadoRequisitoExpedienteId { get; set; }
        public virtual EstadoRequisitoExpediente EstadoRequisitoExpediente { get; set; }
        public int RequisitoExpedienteId { get; set; }
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
        public int ExpedienteAlumnoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumno { get; set; }
        public int TipoRequisitoExpedienteId { get; set; }
        public virtual TipoRequisitoExpediente TipoRequisitoExpediente { get; set; }
        public int? CausaEstadoRequisitoConsolidadaExpedienteId { get; set; }
        public virtual CausaEstadoRequisitoConsolidadaExpediente CausaEstadoRequisitoConsolidadaExpediente { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpedienteDocumento> ConsolidacionesRequisitosExpedientesDocumentos { get; set; }

        public virtual bool EsEstadoValidado()
        {
            return EstadoRequisitoExpedienteId == EstadoRequisitoExpediente.Validado;
        }

        public virtual bool EsValidoParaRechazarValidar()
        {
            return EstadoRequisitoExpedienteId == EstadoRequisitoExpediente.Pendiente
                || EstadoRequisitoExpedienteId == EstadoRequisitoExpediente.Rechazado;
        }

        public virtual bool EsValidoParaEliminar()
        {
            return EstadoRequisitoExpedienteId != EstadoRequisitoExpediente.Validado;
        }

        public virtual List<string> VerificarPropiedadesParaEstadoValidado()
        {
            var errors = new List<string>();
            if (RequisitoExpediente.RequiereDocumentacion
                && !DocumentacionRecibida && !EsDocumentacionFisica
                && !ConsolidacionesRequisitosExpedientesDocumentos.Any())
                errors.Add("El requisito requiere documentación");

            if (ConsolidacionesRequisitosExpedientesDocumentos.Any(cred => !cred.FicheroValidado))
                errors.Add("Todos los documentos asociados deben estar marcados como válidos");

            if (RequisitoExpediente.RequisitosExpedientesDocumentos
                .Any(red => red.RequiereAceptacionAlumno) && !EnviadaPorAlumno)
                errors.Add("Existen documentos asociados al requisito que requieren aceptación del alumno pero la consolidación no está marcada como enviada por alumno");

            if (RequisitoExpediente.RequiereTextoAdicional
                && string.IsNullOrWhiteSpace(Texto))
                errors.Add("El requisito requiere texto adicional y no se ha ingresado el texto en la consolidación");

            if (RequisitoExpediente.CertificadoIdioma
                && (string.IsNullOrWhiteSpace(IdRefIdioma)
                    || string.IsNullOrWhiteSpace(NivelIdioma)))
                errors.Add("El requisito está marcado como certificado idioma y no se ha ingresado el idioma y/o nivel idioma");

            return errors;
        }

        public virtual string VerificarPropiedadesParaActualizar(string texto, string idRefIdioma, string nivelIdioma)
        {
            if (EsEstadoValidado())
                return "No se puede modificar cuando el estado es Validado";

            if (RequisitoExpediente.RequiereTextoAdicional && string.IsNullOrWhiteSpace(texto))
                return "Debe ingresar un texto";

            if (RequisitoExpediente.CertificadoIdioma && string.IsNullOrWhiteSpace(idRefIdioma))
                return "Debe seleccionar el idioma";

            if (RequisitoExpediente.CertificadoIdioma && string.IsNullOrWhiteSpace(nivelIdioma))
                return "Debe ingresar el nivel de idioma";

            return null;
        }
    }
}
