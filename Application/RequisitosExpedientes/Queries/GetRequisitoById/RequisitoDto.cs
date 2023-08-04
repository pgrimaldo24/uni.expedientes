using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitoById
{
    public class RequisitoDto : IMapFrom<RequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; }
        public bool EstaVigente { get; set; }
        public bool RequeridaParaTitulo { get; set; }
        public bool RequiereDocumentacion { get; set; }
        public bool EnviarEmailAlumno { get; set; }
        public bool RequeridaParaPago { get; set; }
        public bool EstaRestringida { get; set; }
        public bool EsSancion { get; set; }
        public bool EsLogro { get; set; }
        public bool EsCertificado { get; set; }
        public bool CertificadoIdioma { get; set; }
        public bool RequiereTextoAdicional { get; set; }
        public bool Bloqueado { get; set; }
        public int? IdRefModoRequerimientoDocumentacion { get; set; }
        public string NombreModoRequerimientoDocumentacion { get; set; }
        public virtual EstadoExpedienteDto EstadoExpediente { get; set; }
        public virtual List<RequisitoExpedienteDocumentoDto> RequisitosExpedientesDocumentos { get; set; }
        public virtual List<RolRequisitoExpedienteDto> RolesRequisitosExpedientes { get; set; }
        public virtual List<RequisitoExpedienteFileTypeDto> RequisitosExpedientesFilesType { get; set; }
        public virtual List<RequisitoExpedienteRequerimientoTituloDto> RequisitosExpedientesRequerimientosTitulos { get; set; }
    }

    public class EstadoExpedienteDto : IMapFrom<EstadoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RequisitoExpedienteDocumentoDto : IMapFrom<RequisitoExpedienteDocumento>
    {
        public int Id { get; set; }
        public string NombreDocumento { get; set; }
        public bool DocumentoObligatorio { get; set; }
        public bool DocumentoEditable { get; set; }
        public bool DocumentoSecurizado { get; set; }
        public bool RequiereAceptacionAlumno { get; set; }
        public string IdRefPlantilla { get; set; }
    }

    public class RolRequisitoExpedienteDto : IMapFrom<RolRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Rol { get; set; }
        public string RolName => Rol == AppConfiguration.KeyAdminRole
            ? AppConfiguration.KeyAdminRoleName : AppConfiguration.KeyGestorRoleName;
    }

    public class RequisitoExpedienteFileTypeDto : IMapFrom<RequisitoExpedienteFileType>
    {
        public int Id { get; set; }
        public string IdRefFileType { get; set; }
    }

    public class RequisitoExpedienteRequerimientoTituloDto : IMapFrom<RequisitoExpedienteRequerimientoTitulo>
    {
        public int Id { get; set; }
        public bool RequiereMatricularse { get; set; }
        public virtual TipoRelacionExpedienteDto TipoRelacionExpediente { get; set; }
    }

    public class TipoRelacionExpedienteDto : IMapFrom<TipoRelacionExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EsLogro { get; set; }
    }
}
