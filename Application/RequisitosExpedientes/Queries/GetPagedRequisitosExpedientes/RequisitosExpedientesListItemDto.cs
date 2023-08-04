using System.Collections.Generic;
using System.Linq;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes
{
    public class RequisitosExpedientesListItemDto : IMapFrom<RequisitoExpediente>
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
        public string ModoRequerimientoDocumentacion { get; set; }

        public bool RequiereMatricularse => RequisitosExpedientesRequerimientosTitulos
                                            .Any(red => red.RequiereMatricularse);
        public RequisitosEstadoExpedienteDto EstadoExpediente { get; set; }
        public ICollection<RequisitoExpedienteRequerimientoTituloDto> RequisitosExpedientesRequerimientosTitulos { get; set; }
    }

    public class RequisitosEstadoExpedienteDto : IMapFrom<EstadoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RequisitoExpedienteRequerimientoTituloDto : IMapFrom<RequisitoExpedienteRequerimientoTitulo>
    {
        public int Id { get; set; }
        public bool RequiereMatricularse { get; set; }
    }
}
