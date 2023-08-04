using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById
{
    public class ConsolidacionRequisitoExpedienteDto : IMapFrom<ConsolidacionRequisitoExpediente>
    {
        public int Id { get; set; }
        public bool EsDocumentacionFisica { get; set; }
        public bool DocumentacionRecibida { get; set; }
        public bool EnviadaPorAlumno { get; set; }
        public string IdRefIdioma { get; set; }
        public string SiglasIdioma { get; set; }
        public string NombreIdioma { get; set; }
        public string NivelIdioma { get; set; }
        public string Texto { get; set; }
        public DateTime? Fecha { get; set; }
        public bool IsEstadoNoProcesada => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.NoProcesado;
        public bool IsEstadoValidada => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.Validado;
        public bool IsEstadoPendiente => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.Pendiente;
        public bool IsEstadoRechazada => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.Rechazado;
        public EstadoRequisitoExpedienteDto EstadoRequisitoExpediente { get; set; }
        public TipoRequisitoExpedienteDto TipoRequisitoExpediente { get; set; }
        public virtual RequisitoExpedienteDto RequisitoExpediente { get; set; }
    }

    public class EstadoRequisitoExpedienteDto : IMapFrom<EstadoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoRequisitoExpedienteDto : IMapFrom<TipoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RequisitoExpedienteDto : IMapFrom<RequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; }
        public bool RequiereDocumentacion { get; set; }
        public bool EsLogro { get; set; }
        public bool CertificadoIdioma { get; set; }
        public bool RequiereTextoAdicional { get; set; }
        public bool Bloqueado { get; set; }
        public bool EstaRestringida { get; set; }
        public ICollection<RequisitoExpedienteDocumentoDto> RequisitosExpedientesDocumentos { get; set; }
        public ICollection<RolRequisitoExpedienteDto> RolesRequisitosExpedientes { get; set; }
    }

    public class RolRequisitoExpedienteDto : IMapFrom<RolRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Rol { get; set; }
        public string RolName => Rol == AppConfiguration.KeyAdminRole
            ? AppConfiguration.KeyAdminRoleName : AppConfiguration.KeyGestorRoleName;
    }

    public class RequisitoExpedienteDocumentoDto : IMapFrom<RequisitoExpedienteDocumento>
    {
        public int Id { get; set; }
        public string NombreDocumento { get; set; }
        public bool DocumentoObligatorio { get; set; }
    }
}
