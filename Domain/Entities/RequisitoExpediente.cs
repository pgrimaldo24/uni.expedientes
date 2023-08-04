using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RequisitoExpediente : Entity<int>
    {
        /// <summary>
        /// Nombre del requisito
        /// </summary>
        [StringLength(250)]
        public string Nombre { get; set; }

        /// <summary>
        /// Número de orden del requisito
        /// </summary>
        public int Orden { get; set; }

        /// <summary>
        /// Descripción del requisito
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Indica si el requisito está o no vigente; valor true, por defecto
        /// </summary>
        public bool EstaVigente { get; set; }

        /// <summary>
        /// Indica si el requisito es de obligado cumplimiento para así poder obtener el título; valor false, por defecto
        /// </summary>
        public bool RequeridaParaTitulo { get; set; }

        /// <summary>
        /// Indica si el cumplimiento del requisito requiere documentación; valor false, por defecto
        /// </summary>
        public bool RequiereDocumentacion { get; set; }

        /// <summary>
        /// Indica si tras el cumplimiento de este hay que enviar un email al alumno para informarle; valor false, por defecto
        /// </summary>
        public bool EnviarEmailAlumno { get; set; }

        /// <summary>
        /// Indica si el requisito es de obligado cumplimiento para así poder realizar el pago de tasas; valor false, por defecto
        /// </summary>
        public bool RequeridaParaPago { get; set; }

        /// <summary>
        /// Indica sí el requisito sólo es visible y gestionable por ciertos roles; valor false, por defecto
        /// </summary>
        public bool EstaRestringida { get; set; }

        /// <summary>
        /// Indica sí el requisito representa o no una sanción; valor false, por defecto
        /// </summary>
        public bool EsSancion { get; set; }

        /// <summary>
        /// Indica sí el requisito representa o no un logro; valor false, por defecto
        /// </summary>
        public bool EsLogro { get; set; }

        /// <summary>
        /// Indica sí el requisito representa o no un certificado; valor false, por defecto
        /// </summary>
        public bool EsCertificado { get; set; }

        /// <summary>
        /// Indica sí el requisito es o no un certificado de idioma; valor false, por defecto; si se indica true, entonces, el anterior es true también
        /// </summary>
        public bool CertificadoIdioma { get; set; }

        /// <summary>
        /// Indica sí el requisito requiere o no de un texto adicional; valor false, por defecto
        /// </summary>
        public bool RequiereTextoAdicional { get; set; }

        /// <summary>
        /// Indica si el requisito está bloqueado y no se puede gestionar; valor false, por defecto. 
        /// Los requisitos bloqueados no se añadirían inicialmente al expediente, aunque, posteriormente, el usuario podría añadirlos manualmente.
        /// Esto es útil para añadir sanciones, logros como premios, documentación adicional, certificado de idioma, etc.
        /// Además, esos requisitos podrán consolidarse varias veces, verificando que su información asociada es diferente
        /// </summary>
        public bool Bloqueado { get; set; }

        /// <summary>
        /// Indica el modo requerido para la documentación
        /// </summary>
        public int? IdRefModoRequerimientoDocumentacion { get; set; }

        // Propiedades de Navegación
        public virtual EstadoExpediente EstadoExpediente { get; set; }
        public virtual ICollection<CausaEstadoRequisitoConsolidadaExpediente> CausasEstadosRequisitosConsolidadasExpediente { get; set; }
        public virtual ICollection<RolRequisitoExpediente> RolesRequisitosExpedientes { get; set; }
        public virtual ICollection<RequisitoExpedienteDocumento> RequisitosExpedientesDocumentos { get; set; }
        public virtual ICollection<RequisitoExpedienteFileType> RequisitosExpedientesFilesType { get; set; }
        public virtual ICollection<RequisitoComportamientoExpediente> RequisitosComportamientosExpedientes { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
        public virtual ICollection<RequisitoExpedienteRequerimientoTitulo> RequisitosExpedientesRequerimientosTitulos { get; set; }

        public virtual void DeleteRolesNoIncluidos(string[] roles)
        {
            var rolesAEliminar = roles == null ? RolesRequisitosExpedientes
                : RolesRequisitosExpedientes.Where(rre => !roles.Contains(rre.Rol));
            foreach (var rol in rolesAEliminar)
            {
                RolesRequisitosExpedientes.Remove(rol);
            }
        }

        public virtual void DeleteFilesTypesNoIncluidos(string[] filesTypes)
        {
            var rolesAEliminar = filesTypes == null ? RequisitosExpedientesFilesType
                : RequisitosExpedientesFilesType.Where(rre => !filesTypes.Contains(rre.IdRefFileType));
            foreach (var rol in rolesAEliminar)
            {
                RequisitosExpedientesFilesType.Remove(rol);
            }
        }
    }
}
